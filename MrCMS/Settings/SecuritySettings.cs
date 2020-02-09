using System.ComponentModel;

namespace MrCMS.Settings
{
    public class SecuritySettings : SystemSettingsBase
    {
        [DisplayName("Log Login Attempts"), AppSettingName("log-login-attempts")]
        public bool LogLoginAttempts { get; set; } = true;

        [DisplayName("Send Login Notification Emails"), AppSettingName("send-login-notification-emails")]
        public bool SendLoginNotificationEmails { get; set; }

        [DisplayName("Two Factor Auth Enabled (requires TwoFactorCodePage adding to work)"), AppSettingName("2fa-enabled")]
        public bool TwoFactorAuthEnabled { get; set; }

        [DisplayName("Send Script Change Notification Emails"), AppSettingName("send-script-change-notification-emails")]
        public bool SendScriptChangeNotificationEmails { get; set; }
    }
}