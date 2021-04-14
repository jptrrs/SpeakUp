using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse.Grammar;

namespace SpeakUp
{
    //Expands the rule constraint check to go beyond the constants
    [HarmonyPatch(typeof(GrammarResolver.RuleEntry), "ValidateConstantConstraints")]
    public class RuleEntry_ValidateConstantConstraints
    {
        private static void Postfix(GrammarResolver.RuleEntry __instance, MethodBase __originalMethod, ref bool __result, Dictionary<string, string> constraints, ref bool ___constantConstraintsChecked, bool ___constantConstraintsValid, bool ___knownUnresolvable)
        {
            if (!__result)
            {
                ___constantConstraintsChecked = false;
                __result = DoubleCheckConstraints(__instance, GrammarResolver_RandomPossiblyResolvableEntry.CurrentRules);
            }
        }

        [HarmonyReversePatch]
        public static bool DoubleCheckConstraints(object instance, Dictionary<string, string> constraints)
        {
            throw new NotImplementedException("To be replaced by Harmony");
        }
    }
}