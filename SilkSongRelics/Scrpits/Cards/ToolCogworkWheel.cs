using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands.Builders;

namespace SilkSongRelics.Scrpits.Cards;
[Pool(typeof(ColorlessCardPool))]
public class ToolCogworkWheel : CustomCardModel
{
    public override string PortraitPath => $"res://SilkSongRelics/ArtWorks/Cards/ToolCogworkWheel.png";
    public override int MaxUpgradeLevel => 4;
	public override bool CanBeGeneratedInCombat => false;
	  public override IEnumerable<CardKeyword> CanonicalKeywords => [(CardKeyword.Exhaust)];
     protected override IEnumerable<DynamicVar> CanonicalVars => 
		[
		new DamageVar(14, ValueProp.Move)
        ];
	public ToolCogworkWheel() : base(0, CardType.Attack, CardRarity.None, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await using AttackContext attackContext = await AttackCommand.CreateContextAsync(base.CombatState, this);
		int attackCount = 1;
		while (attackCount > 0)
		{
			attackCount--;
			IEnumerable<DamageResult> enumerable = await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Damage, base.Owner.Creature, this);
			attackContext.AddHit(enumerable);
			attackCount += enumerable.Count((DamageResult r) => r.WasTargetKilled);
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(7); 
	}
}
