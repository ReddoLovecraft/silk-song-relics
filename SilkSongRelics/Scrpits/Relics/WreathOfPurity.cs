using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Patchouib.Scrpits.Main;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class WreathOfPurity: SilkSongReic,IRightCilckable
{
    public override bool IsUsedUp => usedup;
	bool usedup=false;
    public override RelicRarity Rarity => RelicRarity.Uncommon;
	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
		    usedup=false;
		}
	}
        public async Task OnRightClick(PlayerChoiceContext context)
        {
            if(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom&&!IsUsedUp)
            {
                usedup=true;
				//清除负面效果
				Flash();
				List<PowerModel> debuffs=new List<PowerModel>();
				foreach(PowerModel pm in Owner.Creature.Powers)
				{
					if(pm.Type==PowerType.Debuff)
					{
						debuffs.Add(pm);
					}
				}
				for(int i=debuffs.Count-1;i>=0;i--)
				{
					await PowerCmd.Remove(debuffs[i]);
					debuffs.RemoveAt(i);
				}
            }
        }
}
}
