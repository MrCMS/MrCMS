using MrCMS.Settings;

namespace MrCMS.Website.PushNotifications
{
    public class WebPushSettings : SiteSettingsBase
    {
        public string VapidPrivateKey { get; set; }
        public string VapidPublicKey { get; set; }
        public string VapidSubject { get; set; }

        [MediaSelector]
        public string NotificationIcon { get; set; }
        [MediaSelector]
        public string NotificationBadge { get; set; }

        public override bool RenderInSettings => true;
    }
}