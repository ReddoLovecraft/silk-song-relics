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
using SilkSongRelics.Scrpits.Powers;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class Cogfly: ToolRelic
{
     private int cnt=4;
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
            cnt = 4;
        }
    public override async Task OnRightClick(PlayerChoiceContext context)
     { 
		if (!(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom) || IsUsedUp)
		{
			return;
		}

		bool mp = IsMultiplayerActiveForOwner(context);
		if (mp)
		{
			if (TryQueueNetAction(context, new RightClickNetAction(RightClickNetKind.Cogfly, Id.Entry), out Task? task))
			{
				MegaCrit.Sts2.Core.Logging.Log.Info($"SilkSongRelics: right-click Cogfly queued ({Id.Entry})");
				if (task != null)
				{
					await task;
				}
			}
			else
			{
				MegaCrit.Sts2.Core.Logging.Log.Warn($"SilkSongRelics: right-click Cogfly could not queue net action ({Id.Entry})");
			}
			return;
		}
		else
		{
			MegaCrit.Sts2.Core.Logging.Log.Info($"SilkSongRelics: right-click Cogfly treated as singleplayer ({Id.Entry})");
		}

		Flash();
		ToolCount--;
		await PowerCmd.Apply<CogflyPower>(Owner.Creature, 1, Owner.Creature, null);

     }
    public override RelicRarity Rarity => RelicRarity.Common;
}
}
