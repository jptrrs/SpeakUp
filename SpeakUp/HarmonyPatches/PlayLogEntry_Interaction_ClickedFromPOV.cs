using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace SpeakUp
{
    //Enables the rule validation feedback when on DevMode and clicking on an entry on the pawn log panel.
    [HarmonyPatch(typeof(PlayLogEntry_Interaction), nameof(PlayLogEntry_Interaction.ClickedFromPOV))]
    public static class PlayLogEntry_Interaction_ClickedFromPOV
    {
        private static void Postfix(PlayLogEntry_Interaction __instance)
        {
            if (Prefs.DevMode) RuleEntry_ValidateConstantConstraints.validationFeedback = true;
        }
    }
}