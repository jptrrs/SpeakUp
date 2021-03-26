using HugsLib;
using HugsLib.Settings;

namespace SpeakUp
{
    public class ModBaseSpeakUp : ModBase
    {
        public static SettingHandle<int>
            LinesPerConversation,
            IntervalBetweenLines;

        public ModBaseSpeakUp()
        {
            Settings.EntryName = "Speak Up";
        }

        public override string ModIdentifier
        {
            get
            {
                return "SpeakUp";
            }
        }

        public override void DefsLoaded()
        {
            UpdateSettings();
        }

        public void UpdateSettings()
        {
            LinesPerConversation = Settings.GetHandle<int>("LinesPerConversation", "LinesPerConversation", null, 3);
            IntervalBetweenLines = Settings.GetHandle<int>("TickBetweenLines", "TicksBetweenLines (60 = 1 sec)", null, 60);
        }
    }
}