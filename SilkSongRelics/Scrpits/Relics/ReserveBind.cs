using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Patchouib.Scrpits.Main;
using SilkSong.Scrpits.Relics;

namespace SilkSongRelics.Scrpits.Relics
{
[Pool(typeof(SharedRelicPool))]
public class ReserveBind : SilkSongReic,IRightCilckable
{
	public override bool IsUsedUp => usedup;
	bool usedup=false;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       HoverTipFactory.ForEnergy(this)
        });
	protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
	{
		new EnergyVar(1)
	});
    public override RelicRarity Rarity => RelicRarity.Common;
    public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is RestSiteRoom )
		{
		    usedup=false;
		}
	}
    public  async Task OnRightClick(PlayerChoiceContext context)
        {
            if(Owner.Creature.CombatState.RunState.CurrentRoom is CombatRoom&&!IsUsedUp)
            {
                usedup=true;
				Flash();
             await  PlayerCmd.SetEnergy(Owner.MaxEnergy,Owner);
            }
        }
}
}
