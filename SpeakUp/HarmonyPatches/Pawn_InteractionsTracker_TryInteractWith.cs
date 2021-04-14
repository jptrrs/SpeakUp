using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace SpeakUp
{
    //Compile to probe the used InteractionDef
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith))]
    public static class Pawn_InteractionsTracker_TryInteractWith
    {
        public static void Prefix(Pawn recipient, ref InteractionDef intDef)
        {
            Log.Message($"interacting with {intDef.defName}");
        }
    }
}