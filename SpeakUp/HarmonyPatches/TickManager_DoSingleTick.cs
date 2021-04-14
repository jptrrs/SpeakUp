using HarmonyLib;
using Verse;

namespace SpeakUp
{
    //Cleans up expired talks
    [HarmonyPatch(typeof(TickManager), "DoSingleTick")]
    internal static class TickManager_DoSingleTick
    {
        private static int 
            lastCleanTick = 0,
            cleanInterval = 60; //60 = 1 second.

        private static void Postfix()
        {
            var num = lastCleanTick + cleanInterval;
            if (num < GenTicks.TicksGame)
            {
                DialogManager.CleanUp();
                lastCleanTick = GenTicks.TicksGame;
            }
        }
    }
}