using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.Grammar;

namespace SpeakUp
{
    //Insert custom rules
    [HarmonyPatch(typeof(GrammarResolver), nameof(GrammarResolver.Resolve))]
    class GrammarResolver_Resolve
    {
        private static FieldInfo constantsInfo = AccessTools.Field(typeof(GrammarRequest), "constants");
        private static FieldInfo rulesInfo = AccessTools.Field(typeof(GrammarRequest), "rules");

        public static void Prefix(object __instance, string rootKeyword, GrammarRequest request)
        {
            if (/*__instance is PlayLogEntry_Interaction entry && */rootKeyword == "r_logentry")
            {
                var initiator = PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.lastInitiator;
                var recipient = PlayLogEntry_Interaction_ToGameStringFromPOV_Worker.lastRecipient;
                List<Rule> rules = (List<Rule>)rulesInfo.GetValue(request);
                rules.AddRange(ExtraGrammarUtility.Rules(initiator, recipient));
            }
        }
    }
}
