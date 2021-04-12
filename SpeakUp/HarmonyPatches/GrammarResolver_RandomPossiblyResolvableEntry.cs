using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    //Exposes the rules for future use and warning for invalid keywords found.
    [HarmonyPatch(typeof(GrammarResolver), nameof(GrammarResolver.RandomPossiblyResolvableEntry))]
    public class GrammarResolver_RandomPossiblyResolvableEntry
    {
        public static List<string> CurrentRules;
        public static Dictionary<string, string> CurrentRulesDic = new Dictionary<string, string>();
        private static FieldInfo outputInfo = AccessTools.Field(typeof(Rule_String), nameof(Rule_String.output));

        public static void Prefix(string keyword, Dictionary<string, string> constants, List<string> extraTags, List<string> resolvedTags, Dictionary<string, List<GrammarResolver.RuleEntry>> ___rules, ref GrammarResolver.RuleEntry __result)
        {
            //Warning to catch invalid keywords.
            if (keyword == "r_logentry" && ___rules.TryGetValue(keyword, null).NullOrEmpty())
            {
                Log.Warning($"RandomPossiblyResolvableEntry found bad value for {keyword}.\n" +
                            $"Constants are: {constants.ToStringSafeEnumerable()}\n" +
                            $"Rules are : {___rules.Values.ToStringSafeEnumerable()}\n");
            }

            //Expose current rules to be used later to check constraints.
            if (keyword == "r_logentry")
            {
                CurrentRules = ___rules.Keys.ToList();
                foreach (Rule_String rule in ___rules.Values.SelectMany(x => x).Where(x => x.rule is Rule_String).Select(x => x.rule))
                {
                    if (!CurrentRulesDic.ContainsKey(keyword))
                    {
                        CurrentRulesDic.SetOrAdd(rule.keyword, (string)outputInfo.GetValue(rule));
                    }
                }
                //Log.Message($"DEBUG {___rules.Values.SelectMany(x=>x).Select(x => x.rule.keyword).ToStringSafeEnumerable()}");
            }
        }

        public static void Postfix(GrammarResolver.RuleEntry __result, string keyword, Dictionary<string, string> constants, List<string> extraTags, List<string> resolvedTags)
        {
            //reset exposed rules cache
            if (!CurrentRules.NullOrEmpty()) CurrentRules.Clear();
            if (!CurrentRulesDic.EnumerableNullOrEmpty()) CurrentRulesDic.Clear();
        }
    }
}
