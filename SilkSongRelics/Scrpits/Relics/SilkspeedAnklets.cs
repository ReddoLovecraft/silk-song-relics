using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class SilkspeedAnklets : SilkSongReic
{
    [SavedProperty]
    public int cnt { get; set; } = 0;
	public override bool ShowCounter => true;
    	public override int DisplayAmount
	{
		get
		{
			return cnt;
		}
	}
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        cnt++;
        InvokeDisplayAmountChanged();
        if(cnt>=10)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(Owner.Creature,1,Owner.Creature,null);
            cnt=0;
            InvokeDisplayAmountChanged();
        }
        await base.AfterCardDrawn(choiceContext, card, fromHandDraw);
    }
}
}
