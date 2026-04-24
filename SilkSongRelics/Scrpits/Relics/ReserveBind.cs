using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Patchouib.Scrpits.Main;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class ReserveBind : SilkSongReic,IRightCilckable
{
	private bool usedUp;
	public override bool IsUsedUp => UsedUp;

	[SavedProperty]
	public bool UsedUp
	{
		get => usedUp;
		set
		{
			AssertMutable();
			usedUp = value;
		}
	}
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       HoverTipFactory.ForEnergy(this)
        });
	protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
	{
		new EnergyVar(1)
	});
    public override RelicRarity Rarity => RelicRarity.Common;
    public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is RestSiteRoom )
		{
		    UsedUp=false;
		}
	}
    public  async Task OnRightClick(PlayerChoiceContext context)
        {
			if (!(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom) || IsUsedUp)
			{
				return;
			}

			bool mp = IsMultiplayerActiveForOwner(context);
			if (mp)
			{
				if (TryQueueNetAction(context, new RightClickNetAction(RightClickNetKind.ReserveBind, Id.Entry), out Task? task))
				{
					MegaCrit.Sts2.Core.Logging.Log.Info($"SilkSongRelics: right-click ReserveBind queued ({Id.Entry})");
					if (task != null)
					{
						await task;
					}
				}
				else
				{
					MegaCrit.Sts2.Core.Logging.Log.Warn($"SilkSongRelics: right-click ReserveBind could not queue net action ({Id.Entry})");
				}
				return;
			}
			else
			{
				MegaCrit.Sts2.Core.Logging.Log.Info($"SilkSongRelics: right-click ReserveBind treated as singleplayer ({Id.Entry})");
			}

			UsedUp = true;
			Flash();
			await PlayerCmd.SetEnergy(Owner.MaxEnergy, Owner);
        }
}
}
