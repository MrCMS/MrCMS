using System.Collections.Generic;
using MrCMS.Entities.Multisite;

namespace MrCMS.Messages
{
    public interface IMessageTemplateProvider
    {
        void SaveTemplate(MessageTemplateBase messageTemplate);
        void SaveSiteOverride(MessageTemplateBase messageTemplate, Site site);
        void DeleteSiteOverride(MessageTemplateBase messageTemplate, Site site);
        List<MessageTemplateBase> GetAllMessageTemplates(Site site);
        T GetMessageTemplate<T>(Site site) where T : MessageTemplateBase, new();
    }
}