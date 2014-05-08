using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentDeletedNotification : IOnDocumentDeleted
    {
        private readonly INotificationPublisher _notificationPublisher;

        public DocumentDeletedNotification(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public void Execute(OnDocumentDeletedEventArgs args)
        {
            var message = string.Format("{1} {0} has been deleted.", args.Document.Name, args.Document.GetAdminController());
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}