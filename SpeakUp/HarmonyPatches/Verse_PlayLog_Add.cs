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
            if (!Pawn_InteractionsTracker_InteractionsTrackerTick.running && entry is PlayLogEntry_Interaction playLogEntry)
            {
                //Log.Message($"Ensuing from {entry}.");
                DialogManager.Ensue(playLogEntry);
            }
        }
    }
}