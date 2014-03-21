using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentAddedNotification : IOnDocumentAdded
    {
        private readonly INotificationPublisher _notificationPublisher;

        public DocumentAddedNotification(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public void Execute(OnDocumentAddedEventArgs args)
        {
            var message = string.Format("<a href=\"/Admin/{2}/Edit/{1}\">{0}</a> has been added.", args.Document.Name,
                                        args.Document.Id, args.Document.GetAdminController());
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}