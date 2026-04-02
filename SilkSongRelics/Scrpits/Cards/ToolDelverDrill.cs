using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace SilkSongRelics.Scrpits.Cards;
[Pool(typeof(ColorlessCardPool))]
public class ToolDelverDrill : CustomCardModel
{
    public override string PortraitPath => $"res://SilkSongRelics/ArtWorks/Cards/ToolDelverDrill.png";
    public override int MaxUpgradeLevel => 4;
	public override bool CanBeGeneratedInCombat => false;
	public override IEnumerable<CardKeyword> CanonicalKeywords => [(CardKeyword.Exhaust)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
          (StunIntent.GetStaticHoverTip())
        });
     protected override IEnumerable<DynamicVar> CanonicalVars => 
		[
		new DamageVar(24, ValueProp.Move)
        ];
	public ToolDelverDrill() : base(0, CardType.Attack, CardRarity.None, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
         if(cardPlay.Target!=null&&!cardPlay.Target.Monster.IntendsToAttack)
        {
			await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
			await CreatureCmd.Stun(cardPlay.Target);	
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(12); 
	}
}
