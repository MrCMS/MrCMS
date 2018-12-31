namespace MrCMS.Settings
{
    public class BundlingSettings : SystemSettingsBase
    {
        [AppSettingName("enable-optimisations")]
        public bool EnableOptimisations { get; set; }

        public override bool RenderInSettings
        {
            get { return true; }
        }
    }
}