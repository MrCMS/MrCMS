using System.ComponentModel;

namespace MrCMS.Settings
{
    public class GoogleRecaptchaSettings : SystemSettingsBase
    {
        public GoogleRecaptchaSettings()
        {
            Enabled = false;
            ByPassScore = 0.5;
        }
        public bool Enabled { get; set; }
        public string ApiKey { get; set; }
        public string SiteKey { get; set; }
        public string ProjectId { get; set; }
        public double ByPassScore { get; set; }

        [DisplayName("Is Checkbox Type")]
        public bool IsCheckboxType { get; set; }
        public override bool RenderInSettings => true;
    }
}