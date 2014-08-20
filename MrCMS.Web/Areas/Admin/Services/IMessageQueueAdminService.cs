using MrCMS.Entities.Messaging;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMessageQueueAdminService
    {
        IPagedList<QueuedMessage> GetMessages(MessageQueueQuery searchQuery);
    }
}