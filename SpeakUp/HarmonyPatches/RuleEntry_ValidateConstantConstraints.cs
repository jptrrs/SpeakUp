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

        private static void Postfix(GrammarResolver.RuleEntry __instance, MethodBase __originalMethod, ref bool __result, Dictionary<string, string> constraints, ref bool ___constantConstraintsChecked, ref bool ___constantConstraintsValid, bool ___knownUnresolvable, bool __state)
        {
            bool doublechecked = false;
            var currentRules = GrammarResolver_RandomPossiblyResolvableEntry.CurrentRules;
			var constants = constraints; //see note above
			var actualConstraints = __instance.rule.constantConstraints;
			if (!__result)
            {
				//__result = DoubleCheckConstraints(__instance, currentRules);
				List<Rule.ConstantConstraint> remaining = UnsolvedConstraints(actualConstraints, constants).ToList();
				if (!remaining.NullOrEmpty())
				{
					__result = ValidateConstraintsAgainstRules(remaining, currentRules, ref ___constantConstraintsValid);
					doublechecked = true;
				}
            }
            if (Prefs.LogVerbose && __state)
            {
                string result = __result ? "success" : "failed";
                StringBuilder feedback = new StringBuilder();
                feedback.Append($"{result.ToUpper()} validating constraints for {__instance.rule.keyword}:");
                feedback.AppendInNewLine($"{actualConstraints.Select(x => $"\"{x.key} {x.type.ToString().ToLower()} {x.value}\"").ToStringSafeEnumerable()}");
                if (__result) feedback.AppendInNewLine($"Solved while checking the {(doublechecked ? "custom rules" : "constants")}.");
                feedback.AppendInNewLine($"The rule text is \"{__instance.rule}\".");
                feedback.AppendInNewLine($"\nChecked against {constants.Count()} constants:\n" +
                    $"{(constants.EnumerableNullOrEmpty() ? "none\n" : constants.ToStringFullContents())}" +
                    $"\n...and {currentRules.Count()} rules:\n" +
                    $"{(currentRules.EnumerableNullOrEmpty() ? "none" : currentRules.Select(x => $"{x.Key}: {x.Value.ResolveTags()}").ToLineList())}");
                Log.Message(feedback.ToString());
            }
        }

        //[HarmonyReversePatch]
        //public static bool DoubleCheckConstraints(object instance, Dictionary<string, string> constraints)
        //{
        //    throw new NotImplementedException("To be replaced by Harmony");
        //}

		private static IEnumerable<Rule.ConstantConstraint> UnsolvedConstraints(List<Rule.ConstantConstraint> constraints, Dictionary<string, string> constants)
        {
			return constraints.Where(x => !constants.ContainsKey(x.key));
        }

		private static bool ValidateConstraintsAgainstRules(List<Rule.ConstantConstraint> constraints, List<KeyValuePair<string, string>> rules, ref bool constantConstraintsValid)
		{
			if (constraints != null && !constantConstraintsValid)
			{
				constantConstraintsValid = true;
				for (int i = 0; i < constraints.Count; i++)
				{
					Rule.ConstantConstraint constraint = constraints[i];
					string text = (rules != null) ? rules.FirstOrDefault(x => x.Key == constraint.key).Key : "";
					float num = 0f;
					float num2 = 0f;
					bool ruleIsvalid = !text.NullOrEmpty() && !constraint.value.NullOrEmpty() && float.TryParse(text, out num) && float.TryParse(constraint.value, out num2);
					bool known;
					switch (constraint.type)
					{
						case Rule.ConstantConstraint.Type.Equal:
							known = text.EqualsIgnoreCase(constraint.value);
							break;
						case Rule.ConstantConstraint.Type.NotEqual:
							known = !text.EqualsIgnoreCase(constraint.value);
							break;
						case Rule.ConstantConstraint.Type.Less:
							known = (ruleIsvalid && num < num2);
							break;
						case Rule.ConstantConstraint.Type.Greater:
							known = (ruleIsvalid && num > num2);
							break;
						case Rule.ConstantConstraint.Type.LessOrEqual:
							known = (ruleIsvalid && num <= num2);
							break;
						case Rule.ConstantConstraint.Type.GreaterOrEqual:
							known = (ruleIsvalid && num >= num2);
							break;
						default:
							Log.Error("Unknown ConstantConstraint type: " + constraint.type, false);
							known = false;
							break;
					}
					if (!known)
					{
						constantConstraintsValid = false;
						break;
					}
				}
			}
			return constantConstraintsValid;
		}
	}
}