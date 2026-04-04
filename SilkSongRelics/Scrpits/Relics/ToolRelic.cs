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
using Patchouib.Scrpits.Main;
using Patchoulib.Scrpits.Main;
using SilkSong.Scrpits.Main;
using SilkSongRelics.Scrpits.Main;
using System.Linq;
namespace SilkSong.Scrpits.Relics
{
public abstract class ToolRelic : SilkSongReic, IRightCilckable
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
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       Tools.GetStaticKeyword("Tool")
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