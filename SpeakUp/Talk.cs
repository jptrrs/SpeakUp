using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SpeakUp
{
    public class Talk
    {
        public static int count => ModBaseSpeakUp.LinesPerConversation;
        public static int interval => ModBaseSpeakUp.IntervalBetweenLines;

        public int
            latestReplyCount = 0,
            expireTick = 0;
        public int remainingReplies => ModBaseSpeakUp.LinesPerConversation - latestReplyCount;

        public string Tag;
        public Pawn Initiator, Recipient;

        public Talk(string tag)
        {
            Tag = tag;
            Initiator = PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.lastInitiator;
            Recipient = PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.lastRecipient;
            Reply();
        }

        public void MakeReply(InteractionDef intDef)
        {
            var time = GenTicks.TicksGame + interval;
            expireTick = time + 1;
            latestReplyCount += 1;
            SwapRoles();
            DialogManager.Scheduled.Add(new Statement(Initiator, Recipient, time, intDef, this, latestReplyCount));
        }

        public void Reply()
        {
            if (remainingReplies > 0)
            {
                InteractionDef intDef = DefDatabase<InteractionDef>.GetNamed(Tag);
                if (intDef != null) MakeReply(intDef);
                else Log.Warning($"[SpeakUp] {Initiator} talked about {Tag}, but there isn't an appropriate interactionDef to respond.");
            }
        }

        private void SwapRoles()
        {
            Pawn swapped = Initiator;
            Initiator = Recipient;
            Recipient = swapped;
        }
    }
}
