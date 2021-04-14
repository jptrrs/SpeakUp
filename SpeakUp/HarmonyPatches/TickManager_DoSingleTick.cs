using HarmonyLib;
using Verse;

namespace SpeakUp
{
    using static DialogManager;

    //Cleans up expired talks
    [HarmonyPatch(typeof(TickManager), "DoSingleTick")]
    internal static class TickManager_DoSingleTick
    {
        private static int 
            lastCleaned = 0,
            cleanInterval = 60; //60 = 1 second.

        private static void Postfix()
        {
            if (!CurrentTalks.NullOrEmpty())
            {
                var num = lastCleaned + cleanInterval;
                if (num < GenTicks.TicksGame)
                {
                    CleanUp();
                    lastCleaned = GenTicks.TicksGame;
                }
            }
        }
    }
}