using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace SpeakUp
{
    public static class DialogManager
    {
        public static List<Statement> Scheduled = new List<Statement>();
        private static FieldInfo
            recipientInfo = AccessTools.Field(typeof(PlayLogEntry_Interaction), "recipient"),
            initiatorInfo = AccessTools.Field(typeof(PlayLogEntry_Interaction), "initiator"),
            intDefInfo = AccessTools.Field(typeof(PlayLogEntry_Interaction), "intDef");

        public static int count => ModBaseSpeakUp.LinesPerConversation;
        public static int interval => ModBaseSpeakUp.IntervalBetweenLines;
        public static void Ensue(PlayLogEntry_Interaction initial)
        {
            var tick = GenTicks.TicksGame;
            Pawn initiator = (Pawn)recipientInfo.GetValue(initial);
            Pawn recipient = (Pawn)initiatorInfo.GetValue(initial);
            for (int i = 0; i <= count; i++)
            {
                var time = tick + (i * interval);
                var reply = new Statement(initiator, recipient, time, (InteractionDef)intDefInfo.GetValue(initial));
                Scheduled.Add(reply);
                Swap(ref initiator, ref recipient);
            }
        }

        private static void Swap(ref Pawn one, ref Pawn two)
        {
            Pawn swapped = one;
            one = two;
            two = swapped;
        }
    }
}