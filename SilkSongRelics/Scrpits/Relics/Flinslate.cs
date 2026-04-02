using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
public class Flintslate: ToolRelic
{
    
     private int cnt=3;
        public override void Reset()
        {
            cnt = 3;
        }
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
       static string text = StringHelper.Slugify("Tool");
    	static LocString locString = ToolBox.L10NStatic(text + ".title");
    	static LocString locString2 = ToolBox.L10NStatic(text + ".description");
      static string text2 = StringHelper.Slugify("BurnPower");
    	static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    	static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
        {
       new HoverTip(locString,locString2),
        new HoverTip(locString3,locString4)
        });	
    public override RelicRarity Rarity => RelicRarity.Rare;
   public override async Task OnRightClick(PlayerChoiceContext context)
     { 
        if(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom&&!IsUsedUp)
        {
            Flash();
            ToolCount--;
            InvokeDisplayAmountChanged();
            await PowerCmd.Apply<FlintslatePower>(Owner.Creature,3,Owner.Creature,null);
        }

     }
}
}