using System.ComponentModel;

namespace MrCMS.Settings
{
    public class SecuritySettings : SystemSettingsBase
    {
        [DisplayName("Send Script Change Notification Emails"), AppSettingName("send-script-change-notification-emails")]
        public bool SendScriptChangeNotificationEmails { get; set; }

        public override bool RenderInSettings => true;
    }
}