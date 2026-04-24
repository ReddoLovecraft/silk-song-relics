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
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Patchouib.Scrpits.Main;
using Patchoulib.Scrpits.Main;
using SilkSong.Scrpits.Main;
using SilkSongRelics.Scrpits.Main;
using System.Linq;
using System.Reflection;
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
	public override bool IsAllowed(IRunState runState)
	{
		if (IsMultiplayerRunState(runState))
		{
			return false;
		}
		return true;
	}
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
		if (!(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom) || IsUsedUp)
		{
			return;
		}

		bool mp = IsMultiplayerActiveForOwner(context);
		if (mp)
		{
			if (TryQueueNetAction(context, new RightClickNetAction(RightClickNetKind.ToolRelic, Id.Entry), out Task? task))
			{
				MegaCrit.Sts2.Core.Logging.Log.Info($"SilkSongRelics: right-click ToolRelic queued ({Id.Entry})");
				if (task != null)
				{
					await task;
				}
			}
			else
			{
				MegaCrit.Sts2.Core.Logging.Log.Warn($"SilkSongRelics: right-click ToolRelic could not queue net action ({Id.Entry})");
			}
			return;
		}
		else
		{
			MegaCrit.Sts2.Core.Logging.Log.Info($"SilkSongRelics: right-click ToolRelic treated as singleplayer ({Id.Entry})");
		}

		Flash();
		ToolCount--;
		List<CardModel> list = new List<CardModel>();
		list.Add(ToolCard);
		await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true);
         
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

	private static bool IsMultiplayerRunState(IRunState runState)
	{
		Type t = runState.GetType();
		PropertyInfo? playersProp = t.GetProperty("Players", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		object? players = playersProp?.GetValue(runState);
		if (players is System.Collections.ICollection c)
		{
			return c.Count > 1;
		}
		if (players is System.Collections.IEnumerable e)
		{
			int n = 0;
			foreach (object _ in e)
			{
				n++;
				if (n > 1)
				{
					return true;
				}
			}
		}
		return false;
	}
    }
}
