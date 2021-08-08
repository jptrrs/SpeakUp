using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    //Exposes the rules for use downstream and warning for invalid keywords found.
    [HarmonyPatch(typeof(GrammarResolver), nameof(GrammarResolver.RandomPossiblyResolvableEntry))]
    public class GrammarResolver_RandomPossiblyResolvableEntry
    {
        public static List<KeyValuePair<string, string>> CurrentRules = new List<KeyValuePair<string, string>>();
        private static FieldInfo outputInfo = AccessTools.Field(typeof(Rule_String), nameof(Rule_String.output));

        public static void Prefix(string keyword, Dictionary<string, string> constants, List<string> extraTags, List<string> resolvedTags, Dictionary<string, List<GrammarResolver.RuleEntry>> ___rules, ref GrammarResolver.RuleEntry __result)
        {
            //Expose current constants & rules 
            if (!___rules.TryGetValue(keyword, null).NullOrEmpty())
            {
                if (!constants.EnumerableNullOrEmpty())
                {
                    foreach (var constant in constants)
                    {
                        CurrentRules.Add(constant);
                    }
                }
                foreach (Rule_String rule in ___rules.Values.SelectMany(x => x).Where(x => x.rule is Rule_String).Select(x => x.rule))
                {
                    CurrentRules.Add(new KeyValuePair<string, string>(rule.keyword, (string)outputInfo.GetValue(rule)));
                }
                return;
            }

            //Warning to catch invalid keywords.
            if (Prefs.LogVerbose && Current.ProgramState == ProgramState.Playing)
            {
                Log.Warning($"[SpeakUp] Bad value found for \"{keyword}\". Could be a typo!");
            }
        }

        public static void Postfix(GrammarResolver.RuleEntry __result, string keyword, Dictionary<string, string> constants, List<string> extraTags, List<string> resolvedTags)
        {
            //reset exposed rules cache
            if (!CurrentRules.EnumerableNullOrEmpty()) CurrentRules.Clear();
        }
    }
}
