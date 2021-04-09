using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace SpeakUp
{
    //Hijacks the used IntDef
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith))]
    public static class Pawn_InteractionsTracker_TryInteractWith
    {
        public static void Prefix(Pawn recipient, ref InteractionDef intDef)
        {
            if (intDef == InteractionDefOf.Chitchat)
            {
                // Log.Warning($"replacing {intDef}");
                intDef = TalkDefOf.Test;
            }
        }
    }
}