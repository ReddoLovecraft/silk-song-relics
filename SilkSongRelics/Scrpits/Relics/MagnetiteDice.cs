using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SilkSong.Scrpits.Main;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class MagnetiteDice : SilkSongReic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
     public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
          if(target==base.Owner.Creature)
        {
            //自身受击
            int diceResult=ToolBox.DollDice(base.Owner.Creature, 20);
            if(diceResult!=1&&diceResult!=2)
            {
                amount-=diceResult;
                amount=amount<0?0:amount;
                ToolBox.MyTalk($"命运的骰子已然掷出，出目为{diceResult}！\n本次伤害减免至{amount}", target);
            }
            else
            {
                amount*=2;
                ToolBox.MyTalk($"不好，出目为{diceResult}！本次伤害翻倍！", target);
            }
        }
        else if(target!=null)
        {
            //其他敌人受击
              int diceResult=ToolBox.DollDice(base.Owner.Creature, 20);
            if(diceResult!=1&&diceResult!=2)
            {
                amount+=diceResult;
                amount=amount<0?0:amount;
                ToolBox.MyTalk($"命运的骰子已然掷出，出目为{diceResult}！\n本次伤害增加至{amount}", target);
            }
            else
            {
                  ToolBox.MyTalk($"不好，出目为{diceResult}！本次伤害归零！", target);
                amount=0;
            }
        }
        return amount;
    }
    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }
}
}
