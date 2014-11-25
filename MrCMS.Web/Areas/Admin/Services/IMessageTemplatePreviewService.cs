using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMessageTemplatePreviewService
    {
        MessageTemplateBase GetTemplate(string type);
        QueuedMessage GetPreview(string type, int id);
    }
}