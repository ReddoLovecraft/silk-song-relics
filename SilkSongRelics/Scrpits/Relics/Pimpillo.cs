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
using Patchoulib.Scrpits.Main;
using SilkSong.Scrpits.Main;
using SilkSong.Scrpits.Relics;
using SilkSongRelics.Scrpits.Cards;
using SilkSongRelics.Scrpits.Powers;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class Pimpillo: ToolRelic
{
    public override CardModel ToolCard=> Owner.Creature.CombatState.CreateCard<ToolPimpillo>(Owner);
        private int cnt=4;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
        {
          Tools.GetStaticKeyword("Tool"),
          Tools.GetStaticKeyword("BurnPower")
        });
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
        public override RelicRarity Rarity => RelicRarity.Uncommon;
}
}