using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SilkSong.Scrpits.Main;

namespace SilkSongRelics.Scrpits.Powers
{
    public sealed class FlintslatePower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://SilkSongRelics/ArtWorks/Powers/FP32.png";
        public override string? CustomBigIconPath => "res://SilkSongRelics/ArtWorks/Powers/FP64.png";
		static string text = StringHelper.Slugify("BurnPower");
    	static LocString locString = ToolBox.L10NStatic(text + ".title");
    	static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       new HoverTip(locString,locString2)
        });	
        public FlintslatePower() { }
	 public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!CombatManager.Instance.IsInProgress)
		{
			await Task.CompletedTask;
			return;
		}
		if (target == base.Owner)
		{
			await Task.CompletedTask;
			return;
		}
        if(dealer==null||dealer!=base.Owner)
        {
			await Task.CompletedTask;
			return;
		}
        Flash();
		await PowerCmd.Apply<BurnPower>(target,this.Amount,Owner,null);
        await Task.CompletedTask;
	}
   public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!props.IsPoweredAttack_())
		{
			return 1m;
		}
		if (cardSource == null)
		{
			return 1m;
		}
		if (dealer != base.Owner)
		{
			return 1m;
		}
		if (target == null)
		{
			return 1m;
		}
		return 1.5m;
	}
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (base.Owner.Side != side)
		{
			await PowerCmd.Remove(this);
		}
	}
    
	}

}