using System.Collections.Generic;

namespace MrCMS.Settings
{
    public class AuthRoleSettings : SystemSettingsBase
    {
        [AppSettingName("send-notification-email-roles")]
        public List<int> SendNotificationEmailRoles { get; set; } = new();
    }
}