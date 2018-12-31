using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class WebpageUpdatedNotification : IOnUpdated<Webpage>
    {
        private readonly IDocumentModifiedUser _documentModifiedUser;
        private readonly INotificationPublisher _notificationPublisher;

        public WebpageUpdatedNotification(INotificationPublisher notificationPublisher,
            IDocumentModifiedUser documentModifiedUser)
        {
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
        }

        public void Execute(OnUpdatedArgs<Webpage> args)
        {
            Webpage current = args.Item;
            Webpage original = args.Original;
            if (current == null || original == null)
                return;

            string action = "updated";
            if ((!original.Published && current.Published))
            {
                action = "published";
            }
            else if ((original.Published && !current.Published))
            {
                action = "unpublished";
            }

            string message = string.Format("<a href=\"/Admin/Webpage/Edit/{1}\">{0}</a> has been {2}{3}.",
                current.Name,
                current.Id, action, _documentModifiedUser.GetInfo());
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}