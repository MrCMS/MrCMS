namespace MrCMS.Settings
{
    public class BundlingSettings : SystemSettingsBase
    {
        public bool EnableOptimisations { get; set; }

        public override bool RenderInSettings
        {
            get { return true; }
        }
    }
}