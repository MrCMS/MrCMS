using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class LayoutUpdatedNotification : IOnUpdated<Layout>
    {
        private readonly IGetNotificationModifiedUserInfo _getNotificationModifiedUserInfo;
        private readonly INotificationPublisher _notificationPublisher;

        public LayoutUpdatedNotification(IGetNotificationModifiedUserInfo getNotificationModifiedUserInfo, INotificationPublisher notificationPublisher)
        {
            _getNotificationModifiedUserInfo = getNotificationModifiedUserInfo;
            _notificationPublisher = notificationPublisher;
        }

        public async Task Execute(OnUpdatedArgs<Layout> args)
        {
            var webpage = args.Item;
            string message = string.Format("<a href=\"/Admin/Layout/Edit/{1}\">{0}</a> has been updated{2}.",
                webpage.Name,
                webpage.Id, await _getNotificationModifiedUserInfo.GetInfo());
            await _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }
    }
}