namespace MrCMS.Settings
{
    public class GoogleRecaptchaSettings : SystemSettingsBase
    {
        public bool Enabled { get; set; }
        public string Secret { get; set; }
        public string SiteKey { get; set; }
        public override bool RenderInSettings => true;
    }
}