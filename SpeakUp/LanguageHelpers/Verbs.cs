using System;
using System.Collections.Generic;
using Verse;


namespace SpeakUp
{
    using static Pronoums;
    public class Verbs
    {
        protected static Dictionary<pronoum, string> conjugation = new Dictionary<pronoum, string>();

        public static string Conjugate(person person, Gender gender)
        {
            return conjugation[person.GetPronoum(gender)];
        }

	}
}
