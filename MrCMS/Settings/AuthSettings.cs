using System.ComponentModel;

namespace MrCMS.Settings
{
    public class AuthSettings : SystemSettingsBase
    {
        [DisplayName("Log Login Attempts"), AppSettingName("log-login-attempts")]
        public bool LogLoginAttempts { get; set; } = true;

        [DisplayName("Two Factor Auth Enabled"), AppSettingName("2fa-enabled")]
        public bool TwoFactorAuthEnabled { get; set; }


        public override bool RenderInSettings => true;
    }
}