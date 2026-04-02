using System.Security.Cryptography;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;

namespace SilkSong.Scrpits.Main
{
    public static class ToolBox
    {
        public static int DollDice(Creature player, int sides,int num=1)
        {
            int result=0;
            for(int i=0;i<num;i++)
            {
                Rng rng = player.Player.RunState.Rng.CombatCardGeneration;
                int randomNumber = rng.NextInt(1, sides+1);
                result+=randomNumber;
           }
            return result;
        }
       private const double _defaultTimePerCharacter = 0.08;
	   private const double _minTimeToDisplay = 1.5;
    public static NSpeechBubbleVfx? MyTalk(String TalkText, Creature speaker, double secondsToDisplay = -1.0, VfxColor vfxColor = VfxColor.White)
	{
		if (speaker.IsDead)
		{
			return null;
		}
		if (secondsToDisplay < 0.0)
		{
			secondsToDisplay = (double)TalkText.Length * 0.08;
		}
		if (secondsToDisplay < 1.5)
		{
			secondsToDisplay = 1.5;
		}
		NSpeechBubbleVfx nSpeechBubbleVfx = NSpeechBubbleVfx.Create(TalkText, speaker, secondsToDisplay, vfxColor);
		if (nSpeechBubbleVfx != null)
		{
			NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(nSpeechBubbleVfx);
		}
		return nSpeechBubbleVfx;
	}
	 public static LocString L10NStatic(string entry)
        {
            return new LocString("static_hover_tips", entry);

        }
       
    }
}