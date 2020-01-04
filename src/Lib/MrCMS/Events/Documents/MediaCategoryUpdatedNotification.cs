using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class MediaCategoryUpdatedNotification : OnDataUpdated<MediaCategory>
    {
        private readonly IDocumentModifiedUser _documentModifiedUser;
        private readonly INotificationPublisher _notificationPublisher;

        public MediaCategoryUpdatedNotification(IDocumentModifiedUser documentModifiedUser, INotificationPublisher notificationPublisher)
        {
            _documentModifiedUser = documentModifiedUser;
            _notificationPublisher = notificationPublisher;
        }


        public override async Task Execute(ChangeInfo data)
        {
            string message = string.Format("<a href=\"/Admin/MediaCategory/Edit/{1}\">{0}</a> has been updated{2}.",
                data.Properties.GetValue<string>(nameof(MediaCategory.Name)),
                data.EntityId, _documentModifiedUser.GetInfo());

            await _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}