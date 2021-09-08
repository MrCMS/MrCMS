using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IMessageQueueAdminService
    {
        Task<IPagedList<QueuedMessage>> GetMessages(MessageQueueQuery searchQuery);
        Task<QueuedMessage> GetMessage(int id);
    }
}