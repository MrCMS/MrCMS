using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class WebpageUpdatedNotification : OnDataUpdated<Webpage>
    {
        private readonly IDocumentModifiedUser _documentModifiedUser;
        private readonly INotificationPublisher _notificationPublisher;

        public WebpageUpdatedNotification(INotificationPublisher notificationPublisher,
            IDocumentModifiedUser documentModifiedUser)
        {
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
        }


        public override async Task Execute(ChangeInfo data)
        {
            var publishedChanged = data.PropertiesUpdated.FirstOrDefault(x => x.Name == nameof(Webpage.Published));

            var action = "updated";
            if (publishedChanged != null)
            {
                action = publishedChanged.CurrentValue is bool && (bool)publishedChanged.CurrentValue
                    ? "published"
                    : "unpublished";
            }

            string message = string.Format("<a href=\"/Admin/Webpage/Edit/{1}\">{0}</a> has been {2}{3}.",
                data.Properties[nameof(Webpage.Name)],
                data.EntityId, action, _documentModifiedUser.GetInfo());
            await _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);

        }
    }
}