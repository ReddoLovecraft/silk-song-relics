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
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using SilkSong.Scrpits.Main;
using SilkSongRelics.Scrpits.Powers;

namespace SilkSongRelics.Scrpits.Cards;
[Pool(typeof(ColorlessCardPool))]
public class ToolPimpillo : CustomCardModel
{
    public override string PortraitPath => $"res://SilkSongRelics/ArtWorks/Cards/ToolPimpillo.png";
    public override int MaxUpgradeLevel => 4;
	public override bool CanBeGeneratedInCombat => false;
	public override IEnumerable<CardKeyword> CanonicalKeywords => [(CardKeyword.Exhaust)];
     protected override IEnumerable<DynamicVar> CanonicalVars => 
		[
        new CardsVar(15)
        ];
	static string text = StringHelper.Slugify("BurnPower");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       new HoverTip(locString,locString2)
        });
	public ToolPimpillo() : base(0, CardType.Skill, CardRarity.None, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		foreach(Creature mos in base.CombatState.HittableEnemies)
		{
			if(mos.IsAlive)
			{
				await PowerCmd.Apply<BurnPower>(mos,base.DynamicVars.Cards.BaseValue,Owner.Creature,this);
			}
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(9); 
	}
}
