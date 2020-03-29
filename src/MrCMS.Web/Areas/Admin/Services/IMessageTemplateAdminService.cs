using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Messages;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMessageTemplateAdminService
    {
        Task<List<MessageTemplateInfo>> GetAllMessageTemplateTypesWithDetails();

        Task<MessageTemplate> GetNewOverride(string type);
        Task<MessageTemplate> GetOverride(string type);
        Task<MessageTemplate> GetTemplate(string type);

        Task AddOverride(MessageTemplate messageTemplate);
        Task Save(MessageTemplate messageTemplate);
        Task DeleteOverride(string type);
        Task ImportLegacyTemplate(string type);
    }
}