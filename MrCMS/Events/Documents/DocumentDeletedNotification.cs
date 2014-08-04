using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentDeletedNotification : IOnDeleted<Document>
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IDocumentModifiedUser _documentModifiedUser;

        public DocumentDeletedNotification(INotificationPublisher notificationPublisher, IDocumentModifiedUser documentModifiedUser)
        {
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
        }

        public void Execute(OnDocumentDeletedEventArgs args)
        {
            var message = string.Format("{1} {0} has been deleted{2}.", args.Document.Name,
                args.Document.GetAdminController(), _documentModifiedUser.GetInfo());

            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }

        public void Execute(OnDeletedArgs<Document> args)
        {
            var document = args.Item;
            if (document != null)
            {
                Execute(new OnDocumentDeletedEventArgs(document));
            }
        }
    }
}