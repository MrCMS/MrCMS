using MrCMS.Entities.Documents;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentDeletedNotification : IOnDeleted<Document>
    {
        private readonly IDocumentModifiedUser _documentModifiedUser;
        private readonly INotificationPublisher _notificationPublisher;

        public DocumentDeletedNotification(INotificationPublisher notificationPublisher,
            IDocumentModifiedUser documentModifiedUser)
        {
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
        }

        public void Execute(OnDeletedArgs<Document> args)
        {
            string message = string.Format("{1} {0} has been deleted{2}.", args.Item.Name,
                args.Item.GetAdminController(), _documentModifiedUser.GetInfo());

            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}