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
        public static int DollDice(Creature player, int sides, int num = 1)
        {
            int result = 0;
            for (int i = 0; i < num; i++)
            {
                Rng rng = player.Player.RunState.Rng.CombatCardGeneration;
                int randomNumber = rng.NextInt(1, sides + 1);
                result += randomNumber;
            }
            return result;
        }
        private const double _defaultTimePerCharacter = 0.08;
        private const double _minTimeToDisplay = 1.5;
    }
}