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
public class ToolVoltVessels : CustomCardModel
{
    public override string PortraitPath => $"res://SilkSongRelics/ArtWorks/Cards/ToolVoltVessels.png";
    public override int MaxUpgradeLevel => 4;
	public override bool CanBeGeneratedInCombat => false;
	  public override IEnumerable<CardKeyword> CanonicalKeywords => [(CardKeyword.Exhaust)];
     protected override IEnumerable<DynamicVar> CanonicalVars => 
		[
		new DamageVar(3, ValueProp.Unblockable|ValueProp.Unpowered),
        new CardsVar(6)
        ];
	public ToolVoltVessels() : base(0, CardType.Attack, CardRarity.None, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		for(int i=0;i<DynamicVars.Cards.IntValue;i++)
		{
			Creature creature = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
			if (creature != null)
			{
				await CreatureCmd.Damage(choiceContext,creature, base.DynamicVars.Damage, this);
			}		
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3); 
	}
}
