namespace MrCMS.Entities.People
{
    public class PushNotificationLog : SiteEntity
    {
        public virtual PushSubscription PushSubscription { get; set; }
        public virtual PushNotification PushNotification { get; set; }
    }
}