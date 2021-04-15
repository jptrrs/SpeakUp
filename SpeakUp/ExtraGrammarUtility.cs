using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
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

        public static IEnumerable<Rule> ExtraRules(Pawn initiator, Pawn recipient)
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
            yield return new Rule_String("HOUR", GenLocalDate.HourInteger(initiator).ToString());
            yield return new Rule_String("DAYPERIOD", DayPeriod(initiator));

            //arte ou planta por perto
            foreach (var group in subjects)
            {
                var thing = GenClosest.ClosestThing_Global(initiator.Position, initiator.Map.listerThings.ThingsInGroup(group), lookRadius);
                if (thing != null) yield return new Rule_String($"NEAREST_{group.ToString().ToLower()}", $"{thing.def.label}");
            }
        }

        public static IEnumerable<Rule> ExtraRulesForPawn(string symbol, Pawn pawn)
        {
            //mood
            yield return new Rule_String(symbol + "mood", pawn.needs.mood.CurLevel.ToString()/*.ToStringByStyle(ToStringStyle.PercentZero)*/);

            //pensamentos
            List<Thought> thoughts = new List<Thought>();
            pawn.needs.mood.thoughts.GetAllMoodThoughts(thoughts);
            foreach (var thought in thoughts.Where(x => x.CurStage != null && x.CurStage.description != null))
            {
                yield return new Rule_String(symbol + "thoughtDefName", thought.def.defName);
                yield return new Rule_String(symbol + "thought", thought.CurStage.description);
            }

            //traits
            foreach (var trait in pawn.story.traits.allTraits)
            {
                yield return new Rule_String(symbol + "trait", trait.Label);
            }

            //best skill
            yield return new Rule_String(symbol + "bestSkill", pawn.skills.skills.Aggregate(AccessHighestSkill).def.skillLabel);

            //worst skill
            yield return new Rule_String(symbol + "worstSkill", pawn.skills.skills.Aggregate(AccessWorstSkill).def.skillLabel);

            //higher passion
            yield return new Rule_String(symbol + "higherPassion", pawn.skills.skills.Aggregate(AccessHighestPassion).def.skillLabel);        
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
