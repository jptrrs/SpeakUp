using HarmonyLib;
using Verse;

namespace SpeakUp
{
    //Isolate pawn variables of future use
    [HarmonyPatch(typeof(PlayLogEntry_Interaction), nameof(PlayLogEntry_Interaction.ToGameStringFromPOV_Worker))]
    public static class PlayLogEntry_Interaction_ToGameStringFromPOV_Worker
    {
        public static Pawn lastInitiator, lastRecipient;
        private static void Prefix(Pawn ___initiator, Pawn ___recipient)
        {
            lastInitiator = ___initiator;
            lastRecipient = ___recipient;
        }
        private static void Postfix()
        {
            lastInitiator = (lastRecipient = null);
        }
    }
}