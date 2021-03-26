using UnityEngine;
using Verse;

namespace SpeakUp
{
    //In case we need more elaborate settings
    public class ModNameMod : Mod
    {
        private ModNameSettings settings;

        public ModNameMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<ModNameSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModNameSettings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "SpeakUp".Translate();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }

    public class ModNameSettings : ModSettings
    {
        public const float beautySensitivityReductionDefault = 0.25f;
        public static float BeautySensitivityReduction = beautySensitivityReductionDefault;
        public static float IndoorsNoNaturalLightPenalty = indoorsNoNaturalLightPenaltyDefault;

        // zero for vanilla
        public static bool IsBeautyOn = false;

        public static bool LinkVents = true;
        public static bool LinkWindows = true;
        private const float indoorsNoNaturalLightPenaltyDefault = 3f; //indoors accelerated degradation when not under windows

        public static void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            string label = "IndoorsNoNaturalLightPenalty".Translate() + ": " + IndoorsNoNaturalLightPenalty.ToStringDecimalIfSmall() + "x";
            string desc = ("IndoorsNoNaturalLightPenaltyDesc").Translate();
            listing.Label(label, -1f, desc);
            IndoorsNoNaturalLightPenalty = listing.Slider(IndoorsNoNaturalLightPenalty, 1f, 10f);
            if (IsBeautyOn)
            {
                listing.Gap(12f);
                string label2 = "BeautySensitivityReduction".Translate() + ": " + BeautySensitivityReduction.ToStringPercent();
                string desc2 = ("BeautySensitivityReductionDesc").Translate();
                listing.Label(label2, -1f, desc2);
                BeautySensitivityReduction = listing.Slider(BeautySensitivityReduction, 0f, 1f);
            }
            listing.Gap(12f);
            listing.Label(("LinkOptionsLabel").Translate() + " (" + ("RequiresRestart").Translate() + "):");
            listing.GapLine();
            listing.CheckboxLabeled(("LinkWindowsAndWalls").Translate(), ref LinkWindows);
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.Contains("RimFridge")))

            {
                listing.CheckboxLabeled(("LinkFridgesAndWalls").Translate(), ref LinkVents);
            }
            else
            {
                listing.CheckboxLabeled(("LinkVentsAndWalls").Translate(), ref LinkVents);
            }
            listing.Gap(12f);
            if (listing.ButtonText("Reset", null))
            {
                BeautySensitivityReduction = 0.25f;
                IndoorsNoNaturalLightPenalty = 3f;
                LinkWindows = true;
                LinkVents = true;
            }
            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref IndoorsNoNaturalLightPenalty, "IndoorsNoNaturalLightPenalty", indoorsNoNaturalLightPenaltyDefault);
            Scribe_Values.Look(ref BeautySensitivityReduction, "ModifiedBeautyImpactFactor", beautySensitivityReductionDefault);
            Scribe_Values.Look(ref LinkWindows, "LinkWindows", true);
            Scribe_Values.Look(ref LinkVents, "LinkVents", true);
            base.ExposeData();
        }
    }
}