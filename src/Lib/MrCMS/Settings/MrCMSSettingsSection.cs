using System.Configuration;

namespace MrCMS.Settings
{
    public sealed class MrCMSSettingsSection : ConfigurationSection
    {
        public MrCMSSettingsSection()
        {
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public KeyValueConfigurationCollection Settings => base[""] as KeyValueConfigurationCollection;
    }
}