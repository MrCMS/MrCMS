using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Messages;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMessageTemplateAdminService
    {
        List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails();

        MessageTemplate GetNewOverride(string type);
        MessageTemplate GetOverride(string type);
        MessageTemplate GetTemplate(string type);

        Task AddOverride(MessageTemplate messageTemplate);
        void Save(MessageTemplate messageTemplate);
        void DeleteOverride(string type);
        Task ImportLegacyTemplate(string type);
    }
}