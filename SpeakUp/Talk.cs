using RimWorld;
using Verse;

namespace SpeakUp
{
    using static DialogManager;
    public class Talk
    {
        public static int count => ModBaseSpeakUp.LinesPerConversation;
        public static int interval => ModBaseSpeakUp.IntervalBetweenLines;

        public int
            latestReplyCount = 0,
            expireTick = 0;
        public int remainingReplies => ModBaseSpeakUp.LinesPerConversation - latestReplyCount;

        public Pawn nextInitiator, nextRecipient;

        public Talk(string tag)
        {
            nextInitiator = Initiator;
            nextRecipient = Recipient;
            Reply(tag);
        }

        public void MakeReply(InteractionDef intDef)
        {
            var time = GenTicks.TicksGame + interval;
            expireTick = time + 1;
            latestReplyCount += 1;
            SwapRoles();
            DialogManager.Scheduled.Add(new Statement(nextInitiator, nextRecipient, time, intDef, this, latestReplyCount));
        }

        public void Reply(string tag)
        {
            if (remainingReplies > 0)
            {
                InteractionDef intDef = DefDatabase<InteractionDef>.GetNamed(tag);
                if (intDef != null) MakeReply(intDef);
                else Log.Warning($"[SpeakUp] {nextInitiator} talked about {tag}, but there isn't an appropriate interactionDef to respond.");
            }
        }

        private void SwapRoles()
        {
            Pawn swapped = nextInitiator;
            nextInitiator = nextRecipient;
            nextRecipient = swapped;
        }
    }
}
