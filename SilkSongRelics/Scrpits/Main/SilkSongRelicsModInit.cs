using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace SilkSong.Scrpits.Main
{
[ModInitializer("Init")]
public class SilkSongRelicsModInit
{
    // 初始化函数
    public static void Init()
    {
        var harmony = new Harmony("SilkSongRelics");
        harmony.PatchAll();
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(SilkSongRelicsModInit).Assembly);
        Log.Debug("SilkSongRelics initialized!");
    }
}
}
