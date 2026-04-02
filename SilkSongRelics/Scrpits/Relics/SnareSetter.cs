using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using SilkSong.Scrpits.Main;
using SilkSong.Scrpits.Relics;
using SilkSongRelics.Scrpits.Cards;
using SilkSongRelics.Scrpits.Powers;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class SnareSetter: ToolRelic
{
       private int cnt=5;
    
    [SavedProperty]
    public override int ToolCount
    {
        get{return cnt;}
        set
        {
          AssertMutable();
			    cnt=value;
			    InvokeDisplayAmountChanged();
        }
    }
        public override void Reset()
        {
            cnt = 5;
        }
        public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task OnRightClick(PlayerChoiceContext context)
     { 
        if(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom&&!IsUsedUp)
        {
            Flash();
            ToolCount--;
            InvokeDisplayAmountChanged();
            await PowerCmd.Apply<SnareSetterPower>(Owner.Creature,1,Owner.Creature,null);
        }

     }
}
}