using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    //Exposes the rules for future use and warning for invalid keywords found.
    [HarmonyPatch(typeof(GrammarResolver), nameof(GrammarResolver.RandomPossiblyResolvableEntry))]
    public class GrammarResolver_RandomPossiblyResolvableEntry
    {
        public static Dictionary<string, string> CurrentRules = new Dictionary<string, string>();
        private static FieldInfo outputInfo = AccessTools.Field(typeof(Rule_String), nameof(Rule_String.output));

        public static void Prefix(string keyword, Dictionary<string, string> constants, List<string> extraTags, List<string> resolvedTags, Dictionary<string, List<GrammarResolver.RuleEntry>> ___rules, ref GrammarResolver.RuleEntry __result)
        {
            //Expose current rules to be used later to check constraints.
            if (!___rules.TryGetValue(keyword, null).NullOrEmpty())
            {
                foreach (Rule_String rule in ___rules.Values.SelectMany(x => x).Where(x => x.rule is Rule_String).Select(x => x.rule))
                {
                    if (!CurrentRules.ContainsKey(keyword))
                    {
                        CurrentRules.SetOrAdd(rule.keyword, (string)outputInfo.GetValue(rule));
                    }
                }
                return;
            }

            //Warning to catch invalid keywords.
            if (Current.ProgramState == ProgramState.Playing)
            {
                Log.Error($"[SpeakUp] Bad value found for \"{keyword}\"");
            }
        }

        public static void Postfix(GrammarResolver.RuleEntry __result, string keyword, Dictionary<string, string> constants, List<string> extraTags, List<string> resolvedTags)
        {
            //reset exposed rules cache
            if (!CurrentRules.EnumerableNullOrEmpty()) CurrentRules.Clear();
        }
    }
}
