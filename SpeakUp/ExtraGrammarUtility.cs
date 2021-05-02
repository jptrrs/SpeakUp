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

        public static IEnumerable<Rule> ExtraRules()
        {
            Pawn initiator = DialogManager.Initiator;
            if (initiator == null) yield break;

            //pawn parameters
            if (PawnCheck())
            {
                Pawn recipient = DialogManager.Recipient;
                string initPrefix = "INITIATOR_", reciPrefix = "RECIPIENT_";
                foreach (var rule in ExtraRulesForPawn(initPrefix, initiator, recipient))
                {
                    yield return rule;
                }
                foreach (var rule in ExtraRulesForPawn(reciPrefix, recipient, initiator))
                {
                    yield return rule;
                }
            }

            //climate
            yield return new Rule_String("WEATHER", initiator.Map.weatherManager.CurWeatherPerceived.label);

            //time
            yield return new Rule_String("HOUR", GenLocalDate.HourInteger(initiator).ToString());
            yield return new Rule_String("DAYPERIOD", DayPeriod(initiator));

            //temperature
            yield return new Rule_String("TEMPERATURE", GenTemperature.GetTemperatureForCell(initiator.Position, initiator.Map).ToString());

            //outdoor?
            yield return new Rule_String("OUTDOORS", initiator.Position.UsesOutdoorTemperature(initiator.Map).ToStringYesNo());

            //nearest things
            foreach (var group in subjects)
            {
                var thing = GenClosest.ClosestThing_Global(initiator.Position, initiator.Map.listerThings.ThingsInGroup(group), lookRadius);
                if (thing != null) yield return new Rule_String($"NEAREST_{group.ToString().ToLower()}", $"{thing.def.label}");
            }
        }

        public static IEnumerable<Rule> ExtraRulesForPawn(string symbol, Pawn pawn, Pawn other)
        {
            //THE PAWN'S MINDSTATE:

            //mood
            yield return new Rule_String(symbol + "mood", pawn.needs.mood.CurLevel.ToString());

            //thoughts
            List<Thought> thoughts = new List<Thought>();
            pawn.needs.mood.thoughts.GetAllMoodThoughts(thoughts);
            List<string> texts = new List<string>(); 
            foreach (var thought in thoughts)
            {
                yield return new Rule_String(symbol + "thoughtDefName", thought.def.defName);
                if (thought.CurStage != null)
                {
                    yield return new Rule_String(symbol + "thoughtLabel", thought.CurStage.label);
                    if (!thought.CurStage.description.NullOrEmpty()) texts.Add(thought.CurStage.description);
                }
            }
            yield return new Rule_String(symbol + "thoughtText", texts.RandomElement());

            //opinion
            yield return new Rule_String(symbol + "opinion", pawn.relations.OpinionOf(other).ToString());

            //THE PAWN'S BIO:

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

            //all skills
            foreach (var skill in pawn.skills.skills)
            {
                yield return new Rule_String(symbol + skill.def.label + "_level", skill.levelInt.ToString());
                yield return new Rule_String(symbol + skill.def.label + "_passion", skill.passion.ToString());
            }

            //OTHER PAWN SITUATIONS

            //current activity
            yield return new Rule_String(symbol + "jobDefName", pawn.CurJob.def.defName);
            yield return new Rule_String(symbol + "jobText", pawn.CurJob.GetReport(pawn));

            //seated?
            yield return new Rule_String(symbol + "seated", Seated(pawn).ToStringYesNo());

        }

        private static string DayPeriod(Pawn p)
		{
			int hour = GenLocalDate.HourInteger(p);
			if (hour >= 5 && hour < 12) return dayPeriod.morning.ToString();
			if (hour >= 12 && hour < 18) return dayPeriod.afternoon.ToString();
			if (hour >= 18 && hour < 24) return dayPeriod.evening.ToString();
			else return dayPeriod.night.ToString();
		}

        private static bool PawnCheck()
        {
            return
                DialogManager.Initiator != null &&
                DialogManager.Recipient != null &&

                //Restricted to humanlike, for now:
                DialogManager.Initiator.RaceProps?.Humanlike == true &&
                DialogManager.Recipient.RaceProps?.Humanlike == true;
        }

        private static bool Seated(Pawn p)
        {
            Building edifice = p.Position.GetEdifice(p.Map);
            return edifice != null && edifice.def.category == ThingCategory.Building && edifice.def.building.isSittable;
        }
    }
}
