using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using SilkSong.Scrpits.Main;
using SilkSongRelics.Scrpits.Main;
using System.Linq;
namespace SilkSong.Scrpits.Relics
{
public abstract class ToolRelic : SilkSongReic
{
    private int cnt=0;
    public virtual CardModel ToolCard=>null;
    [SavedProperty]
    public virtual int ToolCount
    {
        get{return cnt;}
        set
        {
            AssertMutable();
			cnt=value;
			InvokeDisplayAmountChanged();
        }
    }
    public void Refresh() { InvokeDisplayAmountChanged(); }
    public override bool IsUsedUp => ToolCount<=0;
    public override bool ShowCounter => true;
    public override int DisplayAmount
	{
		get
		{
			return ToolCount;
		}
	}
     static string text = StringHelper.Slugify("Tool");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       new HoverTip(locString,locString2)
        });
    public virtual async Task OnRightClick(PlayerChoiceContext context)
    {
        //将对应卡牌加入手中
        if(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom&&!IsUsedUp)
        {
            Flash();
            ToolCount--;
            InvokeDisplayAmountChanged();
            List<CardModel> list = new List<CardModel>();
            list.Add(ToolCard);
            await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true);
            }
         
    }
    public virtual void Reset() { }
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
            if (player != base.Owner)
            {
                return false;
            }
            bool flag = true;
            foreach (RestSiteOption option in options) 
            {
                if (option is FixToolOption)
                {
                    flag = false;
                    break;
                }
            }
            if (flag) 
            options.Add(new FixToolOption(player));
            return true;
    }
    }
}