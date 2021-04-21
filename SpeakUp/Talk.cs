using RimWorld;
using Verse;

namespace SpeakUp
{
    using static DialogManager;
    using static ModBaseSpeakUp;
    public class Talk
    {
        public static int count => LinesPerConversation;
        public static int interval => IntervalBetweenLines;

        public int
            latestReplyCount = 0,
            expireTick = 0;
        public int remainingReplies => LinesPerConversation - latestReplyCount;

        public Pawn nextInitiator, nextRecipient;

        private string tagToContinue = "continue";

        public Talk(string tag)
        {
            nextInitiator = Initiator;
            nextRecipient = Recipient;
            Reply(tag);
        }

        public void MakeReply(InteractionDef intDef, bool swap = true)
        {
            var time = GenTicks.TicksGame + interval;
            expireTick = time + 1;
            latestReplyCount += 1;
            if (swap) SwapRoles();
            Scheduled.Add(new Statement(nextInitiator, nextRecipient, time, intDef, this, latestReplyCount));
        }

        public void Reply(string tag)
        {
            if (SameRegionRestriction && Initiator.GetRegion() != Recipient.GetRegion()) return;
            if (remainingReplies > 0)
            {
                bool continuing = tag == tagToContinue;
                InteractionDef intDef = continuing ? lastInteractionDef : DefDatabase<InteractionDef>.GetNamed(tag, false);
                if (intDef != null) MakeReply(intDef, !continuing);
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
