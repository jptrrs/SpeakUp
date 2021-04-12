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
        public Talk Talk;
        public int 
            Timing,
            Iteration;

        public Statement(Pawn emitter, Pawn recipient, int timing, InteractionDef intDef, Talk talk, int iteration)
        {
            Emitter = emitter;
            Reciever = recipient;
            Timing = timing;
            IntDef = intDef;
            Talk = talk;
            Iteration = iteration;
        }
    }
}