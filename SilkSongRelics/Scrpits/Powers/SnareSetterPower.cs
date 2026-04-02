using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SilkSongRelics.Scrpits.Powers
{
    public sealed class SnareSetterPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://SilkSongRelics/ArtWorks/Powers/SSP32.png";
        public override string? CustomBigIconPath => "res://SilkSongRelics/ArtWorks/Powers/SSP64.png";
			protected override IEnumerable<DynamicVar> CanonicalVars => [(new DamageVar(25m, ValueProp.Unpowered))];
        public SnareSetterPower() { }
      public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _, ValueProp props, Creature? dealer, CardModel? __)
	{
		if (target == base.Owner && dealer != null && props.IsPoweredAttack_())
		{
			Flash();
			await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Damage, base.Owner);
			await PowerCmd.Decrement(this);
		}
	}
    }
    

}