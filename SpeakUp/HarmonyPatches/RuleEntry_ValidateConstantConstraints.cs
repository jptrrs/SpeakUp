using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    //Expands the rule constraint check to go beyond the constants
    [HarmonyPatch(typeof(GrammarResolver.RuleEntry), "ValidateConstantConstraints")]
    public class RuleEntry_ValidateConstantConstraints
    {
        //NOTE: Tynan called the parameter for this method "constraints", but actually it means "constants".
        //The real constraints are at rule.constantConstraints. Very confusing!

        private static void Prefix(GrammarResolver.RuleEntry __instance, Dictionary<string, string> constraints, ref bool __state)
        {
            __state = Current.ProgramState == ProgramState.Playing && !__instance.constantConstraintsChecked && __instance.rule.constantConstraints != null;
        }

        private static void Postfix(GrammarResolver.RuleEntry __instance, MethodBase __originalMethod, ref bool __result, Dictionary<string, string> constraints, ref bool ___constantConstraintsChecked, bool ___constantConstraintsValid, bool ___knownUnresolvable, bool __state)
        {
            bool doublechecked = false;
            if (!__result)
            {
                ___constantConstraintsChecked = false;
                __result = DoubleCheckConstraints(__instance, GrammarResolver_RandomPossiblyResolvableEntry.CurrentRules);
                doublechecked = true;
            }
            if (Prefs.LogVerbose && __state)
            {
                var currentRules = GrammarResolver_RandomPossiblyResolvableEntry.CurrentRules;
                var actualConstraints = __instance.rule.constantConstraints;
                string result = __result ? "success" : "failed";
                StringBuilder feedback = new StringBuilder();
                feedback.Append($"{result.ToUpper()} validating constraints for {__instance.rule.keyword}:");
                feedback.AppendInNewLine($"{actualConstraints.Select(x => $"\"{x.key} {x.type.ToString().ToLower()} {x.value}\"").ToStringSafeEnumerable()}");
                if (__result) feedback.AppendInNewLine($"Solved while checking the {(doublechecked ? "custom rules" : "constants")}.");
                feedback.AppendInNewLine($"The rule text is \"{__instance.rule}\".");
                feedback.AppendInNewLine($"\nChecked against {constraints.Count()} constants:\n" +
                    $"{(constraints.EnumerableNullOrEmpty() ? "none\n" : constraints.ToStringFullContents())}" +
                    $"\n...and {currentRules.Count()} rules:\n" +
                    $"{(currentRules.EnumerableNullOrEmpty() ? "none" : currentRules.ToStringFullContents())}");
                Log.Message(feedback.ToString());
            }
        }

        [HarmonyReversePatch]
        public static bool DoubleCheckConstraints(object instance, Dictionary<string, string> constraints)
        {
            throw new NotImplementedException("To be replaced by Harmony");
        }
    }
}