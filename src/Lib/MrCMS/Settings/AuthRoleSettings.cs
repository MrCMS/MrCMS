using System.Collections.Generic;

namespace MrCMS.Settings
{
    public class AuthRoleSettings : SystemSettingsBase
    {
        [AppSettingName("send-notification-email-roles")]
        public List<int> SendNotificationEmailRoles { get; set; } = new List<int>();
        [AppSettingName("2fa-roles")]
        public List<int> TwoFactorAuthRoles { get; set; } = new List<int>();
    }
}