using HarmonyLib;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    using static DialogManager;
    [HarmonyPatch(typeof(GrammarResolver), "TryResolveRecursive")]
    public class GrammarResolver_TryResolveRecursive
    {
        //Ensures the tag/requiredTag functionality works.
        public static void Prefix(GrammarResolver.RuleEntry entry, List<string> resolvedTags)
        {
            if (entry.rule is Rule_String stringRule && !stringRule.tag.NullOrEmpty() && !resolvedTags.Contains(stringRule.tag))
            {
                //Log.Message($"Adding tag {stringRule.tag} to {entry.rule.keyword} with {entry.uses} uses");
                resolvedTags.Add(stringRule.tag);
            }
        }

        //Prepares a reply if entry carries a tag
        public static void Postfix(bool __result, GrammarResolver.RuleEntry entry, List<string> resolvedTags)
        {
            if (__result && entry.rule.keyword == "r_logentry")
            {
                if (!resolvedTags.NullOrEmpty()) Ensue(resolvedTags);
            }
        }

    }
}