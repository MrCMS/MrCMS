namespace MrCMS.Entities.People
{
    public class PushNotificationLog : SiteEntity
    {
        public virtual PushSubscription PushSubscription { get; set; }
        public int PushSubscriptionId { get; set; }
        public virtual PushNotification PushNotification { get; set; }
        public int PushNotificationId { get; set; }
    }
}