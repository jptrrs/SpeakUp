using RimWorld;
using Verse;

namespace SpeakUp
{
    public class Statement
    {
        public Pawn
            Emitter,
            Reciever;

        public InteractionDef IntDef;
        public int Timing;

        public Statement(Pawn emitter, Pawn recipient, int timing, InteractionDef intDef)
        {
            Emitter = emitter;
            Reciever = recipient;
            Timing = timing;
            IntDef = intDef;
        }
    }
}