using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
namespace SilkSong.Scrpits.Relics
{
public abstract class SilkSongReic : CustomRelicModel
{
    
   // protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    public override string PackedIconPath => $"res://SilkSongRelics/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://SilkSongRelics/ArtWorks/Relics/{Id.Entry}.png";
    protected override string BigIconPath => $"res://SilkSongRelics/ArtWorks/Relics/{Id.Entry}.png";
    public virtual async Task OnRightClick(PlayerChoiceContext context){await Task.CompletedTask;}
   
}
}