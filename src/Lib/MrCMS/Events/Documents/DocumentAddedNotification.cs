using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentAddedNotification : OnDataAdded<Document>
    {
        private readonly IRepository<Document> _documentRepository;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IDocumentModifiedUser _documentModifiedUser;

        public DocumentAddedNotification(IRepository<Document> documentRepository, INotificationPublisher notificationPublisher, IDocumentModifiedUser documentModifiedUser)
        {
            _documentRepository = documentRepository;
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
        }

        public void PublishMessage(Document document)
        {
            var message = string.Format("<a href=\"/Admin/{2}/Edit/{1}\">{0}</a> has been added{3}.", document.Name,
                document.Id, document.GetAdminController(), _documentModifiedUser.GetInfo());
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }

        public override async Task Execute(EntityData data)
        {
            var document = await _documentRepository.GetData(data.EntityId);
            PublishMessage(document);
        }
    }
}