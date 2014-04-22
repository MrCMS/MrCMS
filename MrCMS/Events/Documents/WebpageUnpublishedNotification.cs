using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class WebpageUnpublishedNotification : IOnWebpageUnpublished
    {
        private readonly INotificationPublisher _notificationPublisher;

        public WebpageUnpublishedNotification(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public void Execute(OnWebpageUnpublishedEventArgs args)
        {
            var message = string.Format("<a href=\"/Admin/Webpage/Edit/{1}\">{0}</a> has been unpublished.",
                                        args.Webpage.Name, args.Webpage.Id);
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}