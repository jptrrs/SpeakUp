using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SpeakUp
{
    public static class DialogManager
    {
        public static List<Statement> Scheduled = new List<Statement>();
        public static List<Talk> CurrentTalks = new List<Talk>();
        public static Pawn Initiator, Recipient;

        public static void Ensue(List<string> tags)
        {
            foreach (string tag in tags)
            {
                Talk ongoing = CurrentTalks.Where(x => x.nextInitiator == Initiator).FirstOrDefault();
                if(ongoing == null)
                {
                    CurrentTalks.Add(new Talk(tag));
                    return;
                }
                ongoing.Reply(tag);
            }
        }

        public static void CleanUp()
        {
            CurrentTalks.RemoveAll(x => x.expireTick < GenTicks.TicksGame);
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