using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentDeletedNotification : OnDataDeleted<Document>
    {
        private readonly IDocumentModifiedUser _documentModifiedUser;
        private readonly IRepository<Document> _document;
        private readonly INotificationPublisher _notificationPublisher;

        public DocumentDeletedNotification(INotificationPublisher notificationPublisher,
            IDocumentModifiedUser documentModifiedUser,
            IRepository<Document> document)
        {
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
            _document = document;
        }

        public override async Task Execute(EntityData data)
        {
            var document = await _document.GetData(data.EntityId);
            string message = string.Format("{1} {0} has been deleted{2}.", document.Name,
                document.GetAdminController(), _documentModifiedUser.GetInfo());

            await _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }

    }
}