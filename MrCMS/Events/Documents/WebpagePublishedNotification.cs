using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class WebpagePublishedNotification : IOnWebpagePublished
    {
        private readonly INotificationPublisher _notificationPublisher;

        public WebpagePublishedNotification(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public void Execute(OnWebpagePublishedEventArgs args)
        {
            var message = string.Format("<a href=\"/Admin/Webpage/Edit/{1}\">{0}</a> has been published.",
                                        args.Webpage.Name, args.Webpage.Id);
            _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}