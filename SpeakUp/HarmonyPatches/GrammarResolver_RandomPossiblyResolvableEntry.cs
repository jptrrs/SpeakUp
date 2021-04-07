using HarmonyLib;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    //Warning for invalid keywords found.
    [HarmonyPatch(typeof(GrammarResolver), nameof(GrammarResolver.RandomPossiblyResolvableEntry))]
    class GrammarResolver_RandomPossiblyResolvableEntry
    {
        public static void Prefix(string keyword, Dictionary<string, string> constants, Dictionary<string, List<GrammarResolver.RuleEntry>> ___rules)
        {
            if (___rules.TryGetValue(keyword, null).NullOrEmpty())
            {
                Log.Warning($"RandomPossiblyResolvableEntry found bad value for {keyword}.");// \n" +
                    //$"Constants are: {constants.ToStringSafeEnumerable()}\n" +
                    //$"Rules are : {___rules.ToStringSafeEnumerable()}\n");
            }
        }
    }
}
