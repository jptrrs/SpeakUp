using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    //Expands the rule constraint check to go beyond the constants
    [HarmonyPatch(typeof(GrammarResolver.RuleEntry), "ValidateConstantConstraints")]
    public class Test
    {
        private static bool enabled = true;
        private static void Postfix(GrammarResolver.RuleEntry __instance, MethodBase __originalMethod, ref bool __result, Dictionary<string, string> constraints, ref bool ___constantConstraintsChecked, bool ___constantConstraintsValid, bool ___knownUnresolvable)
        {
            if (!__result/* && enabled*/)
            {
                //enabled = false;
                //__result = (bool)__originalMethod.Invoke(__instance, new object[] { GrammarResolver_RandomPossiblyResolvableEntry.CurrentRulesDic });
                //enabled = true;
                //var currentRules = GrammarResolver_RandomPossiblyResolvableEntry.CurrentRules;
                //if (!currentRules.NullOrEmpty()) doubleCheck = DoubleCheckConstraints(__instance.rule, currentRules);
                ___constantConstraintsChecked = false;
                __result = DoubleCheckConstraints(__instance, GrammarResolver_RandomPossiblyResolvableEntry.CurrentRulesDic);
            }
        }

        //private static bool DoubleCheckConstraints(Rule rule, List<string> ruleSet)
        //{
        //    for (int i = 0; i < rule.constantConstraints.Count; i++)
        //    {
        //        Rule.ConstantConstraint constraint = rule.constantConstraints[i];
        //        if (ruleSet.Contains(constraint.key))
        //        {
        //            Log.Message($"Actually, {constraint.key} found on a rule!");
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        [HarmonyReversePatch]
        public static bool DoubleCheckConstraints(object instance, Dictionary<string, string> constraints)
        {
            throw new NotImplementedException("It's a stub");
        }
    }
}