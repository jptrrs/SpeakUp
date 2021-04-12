using HarmonyLib;
using Verse;

namespace SpeakUp
{
    //In case we ditch HugsLib
    [StaticConstructorOnStartup]
    public class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            var harmony = new Harmony("SpeakUp");
            harmony.PatchAll();
            harmony.DEBUG = true;
        }
    }
}