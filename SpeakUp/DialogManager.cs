using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace SpeakUp
{
    public static class DialogManager
    {
        public static List<Statement> Scheduled = new List<Statement>();
        public static List<Talk> CurrentTalks = new List<Talk>();

        private static FieldInfo
            recipientInfo = AccessTools.Field(typeof(PlayLogEntry_Interaction), "recipient"),
            initiatorInfo = AccessTools.Field(typeof(PlayLogEntry_Interaction), "initiator"),
            intDefInfo = AccessTools.Field(typeof(PlayLogEntry_Interaction), "intDef");


        //public static void Ensue(PlayLogEntry_Interaction initial)
        //{
        //    var tick = GenTicks.TicksGame;
        //    Pawn initiator = (Pawn)recipientInfo.GetValue(initial);
        //    Pawn recipient = (Pawn)initiatorInfo.GetValue(initial);
        //    for (int i = 0; i <= count; i++)
        //    {
        //        var time = tick + (i * interval);
        //        var reply = new Statement(initiator, recipient, time, (InteractionDef)intDefInfo.GetValue(initial));
        //        Scheduled.Add(reply);
        //        Swap(ref initiator, ref recipient);
        //    }
        //}

        public static void Ensue(List<string> tags)
        {
            foreach (string tag in tags)
            {
                if (!CurrentTalks.Any(x => x.Tag == tag))
                {
                    CurrentTalks.Add(new Talk(tag));
                    return;
                }
                CurrentTalks.First(x => x.Tag == tag).Reply();
            }
        }

        public static void CleanUp()
        {
            if (!CurrentTalks.EnumerableNullOrEmpty())
            {
                CurrentTalks.RemoveAll(x => x.expireTick < GenTicks.TicksGame);
            }
        }

        public static void FireStatement(Statement statement)
        {
            var intDef = statement.IntDef;
            intDef.ignoreTimeSinceLastInteraction = true; //temporary, bc RW limit is 120 ticks
            statement.Emitter.interactions.TryInteractWith(statement.Reciever, statement.IntDef);
            Scheduled.Remove(statement);
        }
    }
}