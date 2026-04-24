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
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using SilkSong.Scrpits.Main;
using SilkSong.Scrpits.Relics;
using SilkSongRelics.Scrpits.Powers;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class MagmaBell : SilkSongReic
{
    public override RelicRarity Rarity => RelicRarity.Common;
	
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       Tools.GetStaticKeyword("BurnPower")
        });
   public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!CombatManager.Instance.IsInProgress)
		{
			await Task.CompletedTask;
			return;
		}
		if (target==null||target != base.Owner.Creature)
		{
			await Task.CompletedTask;
			return;
		}
        if(dealer==null||dealer==Owner.Creature)
        {
			await Task.CompletedTask;
			return;
		}
        Flash();
		await PowerCmd.Apply<BurnPower>(dealer,2,Owner.Creature,null);
        await Task.CompletedTask;
	}
}
}
