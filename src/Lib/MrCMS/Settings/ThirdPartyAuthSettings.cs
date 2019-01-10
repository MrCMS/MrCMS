using System.ComponentModel;

namespace MrCMS.Settings
{
    public class ThirdPartyAuthSettings : SystemSettingsBase
    {
        [DisplayName("Is Google Login Enabled?"), AppSettingName("google-enabled")]
        public bool GoogleEnabled { get; set; }
        [DisplayName("Google Client Id"), AppSettingName("google-client-id")]
        public string GoogleClientId { get; set; }
        [DisplayName("Google Secret"), AppSettingName("google-client-secret")]
        public string GoogleClientSecret { get; set; }


        [DisplayName("Is Facebook Login Enabled?"), AppSettingName("facebook-enabled")]
        public bool FacebookEnabled { get; set; }
        [DisplayName("Facebook App Id"), AppSettingName("facebook-app-id")]
        public string FacebookAppId { get; set; }
        [DisplayName("Facebook App Secret"), AppSettingName("facebook-app-secret")]
        public string FacebookAppSecret { get; set; }

        [DisplayName("Is LinkedIn Login Enabled?"), AppSettingName("linkedin-enabled")]
        public bool LinkedInEnabled { get; set; }
        [DisplayName("LinkedIn Client Id"), AppSettingName("linkedin-client-id")]
        public string LinkedInClientId { get; set; }
        [DisplayName("LinkedIn Client Secret"), AppSettingName("linkedin-client-secret")]
        public string LinkedInClientSecret { get; set; }
    }
}