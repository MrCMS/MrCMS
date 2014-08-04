using MrCMS.Entities.Documents;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentAddedNotification : IOnAdded<Document>
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IDocumentModifiedUser _documentModifiedUser;

        public DocumentAddedNotification(INotificationPublisher notificationPublisher, IDocumentModifiedUser documentModifiedUser)
        {
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
        }

        public void PublishMessage(Document document)
        {
            var message = string.Format("<a href=\"/Admin/{2}/Edit/{1}\">{0}</a> has been added{3}.", document.Name,
                document.Id, document.GetAdminController(), _documentModifiedUser.GetInfo());
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }

        public void Execute(OnAddedArgs<Document> args)
        {
            var document = args.Item;
            if (document != null)
            {
                PublishMessage(document);
            }
        }
    }
}