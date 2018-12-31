using System.Configuration;

namespace MrCMS.Settings
{
    public sealed class MrCMSSettingsSection : ConfigurationSection
    {
        public MrCMSSettingsSection()
        {
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public KeyValueConfigurationCollection Settings
        {
            get { return base[""] as KeyValueConfigurationCollection; }
        }
    }
}