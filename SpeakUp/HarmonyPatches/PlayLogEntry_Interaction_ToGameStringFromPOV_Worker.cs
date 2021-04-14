using HarmonyLib;
using Verse;

namespace SpeakUp
{
    using static DialogManager;
    //Isolate pawn variables of future use
    [HarmonyPatch(typeof(PlayLogEntry_Interaction), nameof(PlayLogEntry_Interaction.ToGameStringFromPOV_Worker))]
    public static class PlayLogEntry_Interaction_ToGameStringFromPOV_Worker
    {
        private static void Prefix(Pawn ___initiator, Pawn ___recipient)
        {
            Initiator = ___initiator;
            Recipient = ___recipient;
        }
        private static void Postfix()
        {
            Initiator = (Recipient = null);
        }
    }
}