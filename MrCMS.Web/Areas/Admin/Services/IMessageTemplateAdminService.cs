using System.Collections.Generic;
using MrCMS.Messages;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMessageTemplateAdminService
    {
        List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails();

        MessageTemplateBase GetNewOverride(string type);
        MessageTemplateBase GetOverride(string type);
        MessageTemplateBase GetTemplate(string type);

        void AddOverride(MessageTemplateBase messageTemplate);
        void Save(MessageTemplateBase messageTemplate);
        void DeleteOverride(MessageTemplateBase messageTemplate);
    }
}