using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SilkSongRelics.Scrpits.Powers
{
    public sealed class BurnPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://SilkSongRelics/ArtWorks/Powers/BP32.png";
        public override string? CustomBigIconPath => "res://SilkSongRelics/ArtWorks/Powers/BP64.png";
        public BurnPower() { }
       public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Side)
		{
			return;
		}
			await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
			if (base.Owner.IsAlive)
			{
				await PowerCmd.ModifyAmount(this,1,null,null);
			}
			else
			{
				await Cmd.CustomScaledWait(0.1f, 0.25f);
			}
	}
    }
    

}