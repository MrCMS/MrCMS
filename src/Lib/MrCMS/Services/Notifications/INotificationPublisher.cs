using System.Threading.Tasks;
using MrCMS.Entities.Notifications;

namespace MrCMS.Services.Notifications
{
    public interface INotificationPublisher
    {
        Task PublishNotification(string message, PublishType publishType = PublishType.Both,
                                 NotificationType notificationType = NotificationType.All);
    }
}