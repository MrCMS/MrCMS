using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMessageTemplatePreviewService
    {
        MessageTemplate GetTemplate(string type);
        QueuedMessage GetPreview(string type, int id);
    }
}