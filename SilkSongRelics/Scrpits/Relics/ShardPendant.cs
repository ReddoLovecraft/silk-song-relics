using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
public class ShardPendant : SilkSongReic
{
    public override RelicRarity Rarity => RelicRarity.Common;
   public override bool IsAllowed(IRunState runState)
	{
		return IsBeforeAct3TreasureChest(runState);
	}
	public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
	{
		if (player != base.Owner)
		{
			return false;
		}
		if (room == null)
		{
			return false;
		}
		if (!room.RoomType.IsCombatRoom())
		{
			return false;
		}
		if (room.RoomType == RoomType.Boss && player.RunState.CurrentActIndex >= player.RunState.Acts.Count - 1)
		{
			return false;
		}
    CombatRoom combatRoom = room as CombatRoom;
    switch (room.RoomType)
				{
				case RoomType.Monster:
        		rewards.Add(new GoldReward((int)Math.Round((double)((float)combatRoom.Encounter.MinGoldReward * combatRoom.GoldProportion)), 
            (int)Math.Round((double)((float)combatRoom.Encounter.MaxGoldReward * combatRoom.GoldProportion)), player, false));
					break;
				case RoomType.Elite:
					rewards.Add(new GoldReward(combatRoom.Encounter.MinGoldReward, combatRoom.Encounter.MaxGoldReward, player, false));
					break;
				case RoomType.Boss:
					rewards.Add(new GoldReward(combatRoom.Encounter.MinGoldReward, combatRoom.Encounter.MaxGoldReward, player, false));
					break;
				}
		return true;
	}
	public override Task AfterModifyingRewards()
	{
		Flash();
		return Task.CompletedTask;
	}
}
}
