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
using MegaCrit.Sts2.Core.ValueProps;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class WardingBell : SilkSongReic
{
	int addtion=0;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.Static(StaticHoverTip.Block))];
	protected override IEnumerable<DynamicVar> CanonicalVars => [(new BlockVar(3m, ValueProp.Unpowered))];
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
            if (!CombatManager.Instance.IsInProgress)
            {
                await Task.CompletedTask;
                return;
            }
            if (target == null || target != Owner.Creature)
            {
                await Task.CompletedTask;
                return;
            }
            if(dealer==Owner.Creature)
            {
                await Task.CompletedTask;
                return;
            }
            base.DynamicVars.Block.UpgradeValueBy(1);
            addtion++;
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, null);
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
            if (player != base.Owner)
            {
                return;
            }
            base.DynamicVars.Block.UpgradeValueBy(-addtion);
            addtion = 0;
    }
    }
}
