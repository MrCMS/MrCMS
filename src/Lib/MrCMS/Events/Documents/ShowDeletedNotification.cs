using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class ShowDeletedNotification : IOnDeleted<Webpage>, IOnDeleted<MediaCategory>, IOnDeleted<Layout>
    {
        private readonly IGetNotificationModifiedUserInfo _getNotificationModifiedUserInfo;
        private readonly INotificationPublisher _notificationPublisher;

        public ShowDeletedNotification(INotificationPublisher notificationPublisher,
            IGetNotificationModifiedUserInfo getNotificationModifiedUserInfo)
        {
            _notificationPublisher = notificationPublisher;
            _getNotificationModifiedUserInfo = getNotificationModifiedUserInfo;
        }

        public async Task PublishMessage(string name, string type)
        {
            string message = string.Format("{1} {0} has been deleted{2}.", name,
                type, await _getNotificationModifiedUserInfo.GetInfo());

            await _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }

        public async Task Execute(OnDeletedArgs<Webpage> args)
        {
            await PublishMessage(args.Item.Name, "Webpage");
        }

        public async Task Execute(OnDeletedArgs<MediaCategory> args)
        {
            await PublishMessage(args.Item.Name, "Media Category");
        }

        public async Task Execute(OnDeletedArgs<Layout> args)
        {
            await PublishMessage(args.Item.Name, "Layout");
        }
    }
}