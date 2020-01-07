using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Messages
{
    public interface IMessageTemplateProvider
    {
        Task SaveTemplate(MessageTemplate messageTemplate);
        Task SaveSiteOverride(MessageTemplate messageTemplate, int siteId);
        Task DeleteSiteOverride(MessageTemplate messageTemplate, int siteId);
        List<MessageTemplate> GetAllMessageTemplates(int siteId);
        Task<T> GetMessageTemplate<T>(int siteId) where T : MessageTemplate, new();
        MessageTemplate GetNewMessageTemplate(Type type);
    }
}