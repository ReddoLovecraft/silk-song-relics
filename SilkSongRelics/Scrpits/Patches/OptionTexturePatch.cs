using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkSongRelics.Scrpits.Patches
{
    [HarmonyPatch(typeof(RestSiteOption))]
    [HarmonyPatch(nameof(RestSiteOption.Icon), MethodType.Getter)]
    public static class OptionTexturePatch
    {
        public static Texture2D CustomTexture = GD.Load<Texture2D>("res://SilkSongRelics/ArtWorks/UI/option_toolfix.png");

        [HarmonyPrefix]
        static bool Prefix(RestSiteOption __instance, ref Texture2D __result)
        {
            if (__instance.OptionId == "FIX_TOOL" && CustomTexture != null)
            {
                __result = CustomTexture;
                return false; 
            }
            return true; 
        }
    }
}
