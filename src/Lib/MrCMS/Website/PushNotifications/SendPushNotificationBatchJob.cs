using MrCMS.Batching.Entities;

namespace MrCMS.Website.PushNotifications
{
    public class SendPushNotificationBatchJob : BatchJob
    {
        public override string Name => "Send batch notification";
    }
}