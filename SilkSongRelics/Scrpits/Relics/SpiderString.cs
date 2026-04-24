using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class SpiderString : SilkSongReic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
	bool flag=true;
	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
            if (player != base.Owner)
            {
                return;
            }
           this.flag=true;
    }
   	public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
	{
        if(power.Type!=PowerType.Buff)
        {
            return amount;
        }
		if (cardSource == null)
		{
			return amount;
		}
        if(giver!=base.Owner.Creature)
        {
			return amount;
		}
        if(!this.flag)
        {
            return amount;
        }
        this.flag=false;
        Flash();
		return amount * 2m;
	}
}
}
