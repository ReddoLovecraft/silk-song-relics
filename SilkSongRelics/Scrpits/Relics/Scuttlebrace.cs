using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class Scuttlebrace : SilkSongReic
{
    protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
	{
		new DynamicVar("Swift",2)
	});
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<Swift>(base.DynamicVars["Swift"].IntValue);
    public override RelicRarity Rarity => RelicRarity.Uncommon;
   public override async Task AfterObtained()
	{
		foreach (CardModel item in Owner.Creature.Player.Deck.Cards)
		{
            if(item.Type==CardType.Skill)
            {
            CardCmd.Enchant<Swift>(item, 2m);
			NCardEnchantVfx nCardEnchantVfx = NCardEnchantVfx.Create(item);
			if (nCardEnchantVfx != null)
			{
				NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(nCardEnchantVfx);
			}
            }
		}
	}
}
}
