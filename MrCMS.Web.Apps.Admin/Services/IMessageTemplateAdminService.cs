using System.Collections.Generic;
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

        void AddOverride(MessageTemplate messageTemplate);
        void Save(MessageTemplate messageTemplate);
        void DeleteOverride(MessageTemplate messageTemplate);
        void ImportLegacyTemplate(string type);
    }
}