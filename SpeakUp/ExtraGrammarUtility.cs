using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
	//using static Pronoums;
	public static class ExtraGrammarUtility
	{
        private static Func<SkillRecord, SkillRecord, SkillRecord> AccessHighestSkill = (A, B) =>
        {
            int a = A.levelInt;
            int b = B.levelInt;
            if (a == b) return (A.passion >= B.passion) ? A : B;
            return (a > b) ? A : B;
        };

        private static Func<SkillRecord, SkillRecord, SkillRecord> AccessHighestPassion = (A, B) =>
        {
            return (A.passion >= B.passion) ? A : B;
        };

        private static Func<SkillRecord, SkillRecord, SkillRecord> AccessWorstSkill = (A, B) =>
        {
            int a = A.levelInt;
            int b = B.levelInt;
            if (a == b) return (A.passion <= B.passion) ? A : B;
            return (a < b) ? A : B;
        };

        private static float lookRadius = 5f;

        private static ThingRequestGroup[] subjects = { ThingRequestGroup.Art, ThingRequestGroup.Plant };

        public enum dayPeriod { morning, afternoon, evening, night }

        public static IEnumerable<Rule> ExtraRulesForPawn(string symbol, Pawn pawn)
        {
            //string Iam = Verb_ToBe.Conjugate(person, pawn.gender);
            //string my = person.GetPronoum(pawn.gender).Possessive();

            //mood
            yield return new Rule_String(symbol + "mood", pawn.needs.mood.CurLevel.ToStringByStyle(ToStringStyle.PercentZero));

            //pensamento (aleatório)
            yield return new Rule_String(symbol + "thought", pawn.needs.mood.thoughts.memories.Memories.Where(x => x.CurStage != null && x.CurStage.description != null).RandomElement().CurStage.description);

            //trait (aleatório)
            yield return new Rule_String(symbol + "trait", pawn.story.traits.allTraits.RandomElement().Label);

            //best skill
            yield return new Rule_String(symbol + "bestSkill", pawn.skills.skills.Aggregate(AccessHighestSkill).def.skillLabel);

            //worst skill
            yield return new Rule_String(symbol + "worstSkill", pawn.skills.skills.Aggregate(AccessWorstSkill).def.skillLabel);

            //higher passion
            yield return new Rule_String(symbol + "higherPassion", pawn.skills.skills.Aggregate(AccessHighestPassion).def.skillLabel);        
        }

        public static IEnumerable<Rule> Rules(Pawn initiator, Pawn recipient)
        {
			string initPrefix = "INITIATOR_", reciPrefix = "RECIPIENT_";

			foreach (var rule in ExtraRulesForPawn(initPrefix, initiator/*, person.firstSingular*/))
			{
				yield return rule;
			}
			foreach (var rule in ExtraRulesForPawn(reciPrefix, recipient/*, person.secondSingular*/))
			{
				yield return rule;
			}

			//clima
			yield return new Rule_String("WEATHER", initiator.Map.weatherManager.CurWeatherPerceived.label);

			//hora 
			yield return new Rule_String("TIME", DayPeriod(initiator));

			//arte ou planta por perto
			foreach (var group in subjects)
            {
				var thing = GenClosest.ClosestThing_Global(initiator.Position, initiator.Map.listerThings.ThingsInGroup(group), lookRadius);
				if (thing != null) yield return new Rule_String($"NEAREST_{group.ToString().ToLower()}", $"{thing.def.label}");
			}
		}

		private static string DayPeriod(Pawn p)
		{
			int hour = GenLocalDate.HourInteger(p);
			if (hour >= 5 && hour < 12) return dayPeriod.morning.ToString();
			if (hour >= 12 && hour < 18) return dayPeriod.afternoon.ToString();
			if (hour >= 18 && hour < 24) return dayPeriod.evening.ToString();
			else return dayPeriod.night.ToString();
		}
	}
}
