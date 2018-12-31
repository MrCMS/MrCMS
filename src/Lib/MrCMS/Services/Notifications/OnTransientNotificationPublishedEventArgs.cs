using MrCMS.Entities.Notifications;

namespace MrCMS.Services.Notifications
{
    public class OnTransientNotificationPublishedEventArgs
    {
        public OnTransientNotificationPublishedEventArgs(Notification notification)
        {
            Notification = notification;
        }

        public Notification Notification { get; set; }
    }
}