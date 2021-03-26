using HarmonyLib;
using Verse;

namespace SpeakUp
{
    //Ensues a conversation from a regular log entry.
    [HarmonyPatch(typeof(PlayLog), "Add")]
    internal static class Verse_PlayLog_Add
    {
        private static void Postfix(LogEntry entry)
        {
            if (!Pawn_InteractionsTracker_InteractionsTrackerTick.running)
            {
                DialogManager.Ensue((PlayLogEntry_Interaction)entry);
            }
        }
    }
}