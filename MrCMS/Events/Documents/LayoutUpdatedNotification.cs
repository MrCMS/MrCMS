using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class LayoutUpdatedNotification : IOnUpdated<Layout>
    {
        private readonly IDocumentModifiedUser _documentModifiedUser;
        private readonly INotificationPublisher _notificationPublisher;

        public LayoutUpdatedNotification(IDocumentModifiedUser documentModifiedUser, INotificationPublisher notificationPublisher)
        {
            _documentModifiedUser = documentModifiedUser;
            _notificationPublisher = notificationPublisher;
        }

        public void Execute(OnUpdatedArgs<Layout> args)
        {
            var webpage = args.Item;
            string message = string.Format("<a href=\"/Admin/Layout/Edit/{1}\">{0}</a> has been updated{2}.",
                webpage.Name,
                webpage.Id, _documentModifiedUser.GetInfo());
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}