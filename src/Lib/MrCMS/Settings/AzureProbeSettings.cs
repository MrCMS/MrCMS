using System;

namespace MrCMS.Settings
{
    public class AzureProbeSettings : SystemSettingsBase
    {
        public AzureProbeSettings()
        {
            Key = "probe";
            Password = Guid.NewGuid().ToString();
        }

        [AppSettingName("azure-probe-key")]
        public string Key { get; set; }

        [AppSettingName("azure-probe-password")]
        public string Password { get; set; }

        public override bool RenderInSettings
        {
            get { return true; }
        }
    }
}