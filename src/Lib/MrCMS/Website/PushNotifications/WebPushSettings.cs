using MrCMS.Settings;

namespace MrCMS.Website.PushNotifications
{
    public class WebPushSettings : SiteSettingsBase
    {
        public string VapidPrivateKey { get; set; }
        public string VapidPublicKey { get; set; }
        public string VapidSubject { get; set; }
        public string DefaultNotificationTitle { get; set; }

        [MediaSelector]
        public string DefaultNotificationIcon { get; set; }
        [MediaSelector]
        public string DefaultNotificationBadge { get; set; }

        public override bool RenderInSettings => true;
        public string SubscriptionConfirmationMessage { get; set; } = "Thank you for subscribing";
        
        public bool LogNotifications { get; set; } = true;
    }
}