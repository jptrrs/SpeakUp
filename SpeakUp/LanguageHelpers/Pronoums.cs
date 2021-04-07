using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SpeakUp
{
    public static class Pronoums
    {
        public enum person { firstSingular, secondSingular, thirdSingular, firstPlural, secondPlural, thirdPlural }
        public enum pronoum { I, You, He, She, It, We, They };

        public static Dictionary<pronoum, string> possessive = new Dictionary<pronoum, string>()
        {
            { pronoum.I, "my" },
            { pronoum.You, "your" },
            { pronoum.He, "his" },
            { pronoum.She, "her" },
            { pronoum.It, "its" },
            { pronoum.We, "our" },
            { pronoum.They, "their" }
        };

        private static Func<Gender, pronoum> thirdPerson = (gender) =>
        {
            switch (gender)
            {
                case Gender.Female:
                    return pronoum.She;
                case Gender.Male:
                    return pronoum.He;
                case Gender.None:
                default:
                    return pronoum.It;
            }
        };

        public static pronoum GetPronoum(this person person, Gender gender)
        {
            switch (person)
            {
                case person.firstSingular:
                default:
                    return pronoum.I;
                case person.secondSingular:
                case person.secondPlural:
                    return pronoum.You;
                case person.thirdSingular:
                    return thirdPerson(gender);
                case person.firstPlural:
                    return pronoum.We;
                case person.thirdPlural:
                    return pronoum.They;
            }
        }

        public static string Possessive(this pronoum pronoum)
        {
			return possessive[pronoum];
        }
	}
}
