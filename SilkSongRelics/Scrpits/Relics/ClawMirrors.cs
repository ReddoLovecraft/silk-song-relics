using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Patchouib.Scrpits.Main;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class ClawMirrors : SilkSongReic,IRightCilckable
{
	public override bool IsUsedUp => usedup;
	bool usedup=false;
	protected override IEnumerable<DynamicVar> CanonicalVars => [(new DamageVar(3m, ValueProp.Unpowered))];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(StunIntent.GetStaticHoverTip())];
    public override RelicRarity Rarity => RelicRarity.Rare;
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
				Flash();
                foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies)
                {
					if(mos.IsAlive)
					await CreatureCmd.Stun(mos);	
				}
				
            }
        }
		public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Creature.Side)
		{
			IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(base.Owner).Cards;
			if (cards.Count != 0)
			{
				for(int i=0;i<cards.Count;i++)
				{
				Flash();
				Creature creature = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
				if (creature != null)
				{
				VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_attack_blunt");
				await CreatureCmd.Damage(choiceContext, creature, base.DynamicVars.Damage, base.Owner.Creature);
				}
				}
			}
		}
	}
}
}
