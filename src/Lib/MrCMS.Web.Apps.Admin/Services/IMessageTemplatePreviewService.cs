using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMessageTemplatePreviewService
    {
        Task<MessageTemplate> GetTemplate(string type);
        Task<QueuedMessage> GetPreview(string type, int id);
    }
}