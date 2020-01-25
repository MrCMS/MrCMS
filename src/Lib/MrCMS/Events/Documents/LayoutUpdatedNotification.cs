using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class LayoutUpdatedNotification : OnDataUpdated<Layout>
    {
        private readonly IDocumentModifiedUser _documentModifiedUser;
        private readonly INotificationPublisher _notificationPublisher;

        public LayoutUpdatedNotification(IDocumentModifiedUser documentModifiedUser, INotificationPublisher notificationPublisher)
        {
            _documentModifiedUser = documentModifiedUser;
            _notificationPublisher = notificationPublisher;
        }


        public override async Task Execute(ChangeInfo data)
        {
            string message = string.Format("<a href=\"/Admin/Layout/Edit/{1}\">{0}</a> has been updated{2}.",
                data.Properties[nameof(Layout.Name)],
                data.EntityId, _documentModifiedUser.GetInfo());
            await _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}