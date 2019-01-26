using MrCMS.Entities.Notifications;

namespace MrCMS.Services.Notifications
{
    public class OnPersistentNotificationPublishedEventArgs
    {
        public OnPersistentNotificationPublishedEventArgs(Notification notification)
        {
            Notification = notification;
        }

        public Notification Notification { get; set; }
    }
}