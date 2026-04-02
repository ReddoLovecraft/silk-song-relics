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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class SnitchPick : SilkSongReic
{
	int deathCount = 0;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [( HoverTipFactory.FromPower<MinionPower>())];
    public override RelicRarity Rarity => RelicRarity.Rare;
	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (target.Side != base.Owner.Creature.Side&&!target.HasPower<MinionPower>())
		{
			Flash();
			deathCount++;
		}
	}
	 public override Task AfterCombatEnd(CombatRoom room)
        {
			for(int i = 0;i<deathCount;i++)
            room.AddExtraReward(Owner, new RelicReward(Owner));
			deathCount=0;
            return Task.CompletedTask;
        }
}
}
