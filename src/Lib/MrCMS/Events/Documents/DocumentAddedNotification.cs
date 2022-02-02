using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;

namespace MrCMS.Events.Documents
{
    public class DocumentAddedNotification : IOnAdded<Webpage>, IOnAdded<Layout>, IOnAdded<MediaCategory>
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IDocumentModifiedUser _documentModifiedUser;

        public DocumentAddedNotification(INotificationPublisher notificationPublisher,
            IDocumentModifiedUser documentModifiedUser)
        {
            _notificationPublisher = notificationPublisher;
            _documentModifiedUser = documentModifiedUser;
        }

        public async Task PublishMessage(string name, int id, string controller)
        {
            var message = string.Format("<a href=\"/Admin/{2}/Edit/{1}\">{0}</a> has been added{3}.", name,
                id, controller, await _documentModifiedUser.GetInfo());
            await _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
        }

        public async Task Execute(OnAddedArgs<Webpage> args)
        {
            var webpage = args.Item;
            if (webpage != null)
            {
                await PublishMessage(webpage.Name, webpage.Id, "Webpage");
            }
        }

        public async Task Execute(OnAddedArgs<Layout> args)
        {
            var layout = args.Item;
            if (layout != null)
            {
                await PublishMessage(layout.Name, layout.Id, "Layout");
            }
        }

        public async Task Execute(OnAddedArgs<MediaCategory> args)
        {
            var category = args.Item;
            if (category != null)
            {
                await PublishMessage(category.Name, category.Id, "MediaCategory");
            }
        }
    }
}