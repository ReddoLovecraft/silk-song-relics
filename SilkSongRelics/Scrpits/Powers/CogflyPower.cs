using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SilkSongRelics.Scrpits.Powers
{
    public sealed class CogflyPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://SilkSongRelics/ArtWorks/Powers/CP32.png";
        public override string? CustomBigIconPath => "res://SilkSongRelics/ArtWorks/Powers/CP64.png";
        public CogflyPower() { }
	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
	{
		if (player == base.Owner.Player)
		{
			for(int i=0;i<base.Amount;i++)
			{
			Flash();
			IReadOnlyList<Creature> hittableEnemies = base.CombatState.HittableEnemies;
			if (hittableEnemies.Count != 0)
			{
				Creature item = base.Owner.Player.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
				await CreatureCmd.Damage(choiceContext, new List<Creature>() { item }, 3, ValueProp.Unpowered, null, null);
			}
			}
		}
	}
    }
    

}