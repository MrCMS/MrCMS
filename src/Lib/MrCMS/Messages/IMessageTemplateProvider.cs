using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Messages
{
    public interface IMessageTemplateProvider
    {
        Task SaveTemplate(MessageTemplate messageTemplate);
        Task SaveSiteOverride(MessageTemplate messageTemplate, Site site);
        Task DeleteSiteOverride(MessageTemplate messageTemplate, Site site);
        Task<List<MessageTemplate>> GetAllMessageTemplates(Site site);
        Task<T> GetMessageTemplate<T>(Site site) where T : MessageTemplate, new();
        Task<MessageTemplate> GetNewMessageTemplate(Type type);
    }
}