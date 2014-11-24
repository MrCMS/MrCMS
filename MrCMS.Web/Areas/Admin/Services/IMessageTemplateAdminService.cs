using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMessageTemplateAdminService
    {
        void Save(MessageTemplateBase messageTemplate);
        List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails();
        MessageTemplateBase GetNewOverride(string type);
        void AddOverride(MessageTemplateBase messageTemplate);
        MessageTemplateBase Reset(MessageTemplateBase messageTemplate);
        List<string> GetTokens(MessageTemplateBase messageTemplate);
        T Get<T>() where T : MessageTemplateBase, new();
        string GetPreview(MessageTemplateBase messageTemplate, int itemId);
        MessageTemplateBase GetOverride(string type);
        void DeleteOverride(MessageTemplateBase messageTemplate);
        MessageTemplateBase GetTemplate(string type);
    }
}