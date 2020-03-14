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
        Task<List<MessageTemplate>> GetAllMessageTemplates(int siteId);
        Task<T> GetMessageTemplate<T>(int siteId) where T : MessageTemplate, new();
        Task<T> GetNewMessageTemplate<T>() where T : MessageTemplate, new();
        Task<MessageTemplate> GetNewMessageTemplate(Type type);
    }
}