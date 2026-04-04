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
using Patchoulib.Scrpits.Main;
using SilkSong.Scrpits.Main;
using SilkSong.Scrpits.Relics;
using SilkSongRelics.Scrpits.Powers;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class WispfireLantern: SilkSongReic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
        {
        Tools.GetStaticKeyword("BurnPower"),
       HoverTipFactory.ForEnergy(this)
        });
      protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
	{
		new EnergyVar(1)
	});
       public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != CombatSide.Player)
            {
                return;
            }
           if (Owner.PlayerCombatState.Energy > 0) 
            { 
				for(int i=0;i<Owner.PlayerCombatState.Energy;i++)
			  {
				Creature creature = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
				if (creature != null)
				{
					await PowerCmd.Apply<BurnPower>(creature,5,Owner.Creature,null);
				}
			  }
            }
        }
}
}
