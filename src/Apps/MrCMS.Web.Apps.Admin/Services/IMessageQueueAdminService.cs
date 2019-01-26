using MrCMS.Entities.Messaging;
using MrCMS.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMessageQueueAdminService
    {
        IPagedList<QueuedMessage> GetMessages(MessageQueueQuery searchQuery);
        QueuedMessage GetMessageBody(int id);
    }
}