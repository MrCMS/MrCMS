using MrCMS.Entities.Notifications;

namespace MrCMS.Services.Notifications
{
    public interface INotificationPublisher
    {
        void PublishNotification(string message, PublishType publishType = PublishType.Both,
                                 NotificationType notificationType = NotificationType.All);
    }
}