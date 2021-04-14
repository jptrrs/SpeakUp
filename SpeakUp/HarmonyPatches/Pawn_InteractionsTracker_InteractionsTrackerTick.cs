using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace SpeakUp
{
    //Fires a new line if scheduled.
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.InteractionsTrackerTick))]
    public static class Pawn_InteractionsTracker_InteractionsTrackerTick
    {
        public static void Postfix(Pawn ___pawn)
        {
            if (___pawn.RaceProps.Humanlike && ___pawn.interactions != null)
            {
                var tick = GenTicks.TicksGame;
                var statement = DialogManager.Scheduled.Where(x => x.Timing <= tick && x.Emitter == ___pawn).FirstOrDefault();
                if (statement != null) DialogManager.FireStatement(statement);
            }
        }
    }
}