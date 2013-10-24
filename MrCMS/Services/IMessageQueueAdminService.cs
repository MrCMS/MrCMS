using MrCMS.Entities.Messaging;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Services
{
    public interface IMessageQueueAdminService
    {
        IPagedList<QueuedMessage> GetMessages(MessageQueueQuery searchQuery);
    }
}