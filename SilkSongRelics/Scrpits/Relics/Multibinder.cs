using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class Multibinder : SilkSongReic
{
  protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
	{
		new EnergyVar(1)
	});

	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
	{
		HoverTipFactory.ForEnergy(this)
	});
		public override async Task AfterEnergyResetLate(Player player)
	{
		if (player == base.Owner)
		{
		   await CreatureCmd.Heal(Owner.Creature,3m);
		}
	}
    public override RelicRarity Rarity => RelicRarity.Rare;
		public override decimal ModifyHandDraw(Player player, decimal cardsToDraw)
	{
		if (player != base.Owner)
		{
			return cardsToDraw;
		}
		return cardsToDraw - 1;
	}
   
}
}
