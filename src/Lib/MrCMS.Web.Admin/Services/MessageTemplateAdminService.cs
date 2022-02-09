using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Web.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class MessageTemplateAdminService : IMessageTemplateAdminService
    {
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly ISession _session;

        public MessageTemplateAdminService(IMessageTemplateProvider messageTemplateProvider,
            ICurrentSiteLocator siteLocator,
            ISession session)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _siteLocator = siteLocator;
            _session = session;
        }

        public async Task<List<MessageTemplateInfo>> GetAllMessageTemplateTypesWithDetails()
        {
            var site = _siteLocator.GetCurrentSite();
            List<MessageTemplate> templates = await _messageTemplateProvider.GetAllMessageTemplates(site);
            IList<string> legacyMessageTemplateTypes =
                await _session.QueryOver<LegacyMessageTemplate>()
                    .Where(template => !template.Imported)
                    .Select(template => template.MessageTemplateType)
                    .Cacheable()
                    .ListAsync<string>();
            return templates.Select(template =>
            {
                Type type = template.GetType();
                return new MessageTemplateInfo
                {
                    Type = type,
                    IsOverride = template.SiteId.HasValue,
                    CanPreview = CanPreview(template),
                    IsEnabled = !template.IsDisabled,
                    LegacyTemplateExists =
                        legacyMessageTemplateTypes.Contains(type.FullName, StringComparer.InvariantCultureIgnoreCase)
                };
            }).ToList();
        }

        public async Task<MessageTemplate> GetNewOverride(string type)
        {
            Type typeByName = TypeHelper.GetTypeByName(type);
            MessageTemplate messageTemplateBase = await _messageTemplateProvider.GetNewMessageTemplate(typeByName);
            if (messageTemplateBase == null) return null;
            var site = _siteLocator.GetCurrentSite();
            messageTemplateBase.SiteId = site.Id;
            return messageTemplateBase;
        }

        public async Task AddOverride(MessageTemplate messageTemplate)
        {
            var site = _siteLocator.GetCurrentSite();
            await _messageTemplateProvider.SaveSiteOverride(messageTemplate, site);
        }

        public async Task<MessageTemplate> GetOverride(string type)
        {
            MessageTemplate messageTemplateBase = await GetTemplate(type);
            if (messageTemplateBase != null && messageTemplateBase.SiteId.HasValue)
                return messageTemplateBase;
            return null;
        }


        public async Task DeleteOverride(string type)
        {
            var messageTemplate = await GetOverride(type);
            if (messageTemplate == null)
                return;
            var site = _siteLocator.GetCurrentSite();
            await _messageTemplateProvider.DeleteSiteOverride(messageTemplate, site);
        }

        public async Task ImportLegacyTemplate(string type)
        {
            var legacyMessageTemplate = await _session.QueryOver<LegacyMessageTemplate>()
                .Where(template => template.MessageTemplateType == type)
                .Take(1)
                .SingleOrDefaultAsync();
            if (legacyMessageTemplate == null)
                return;
            var messageTemplate = await GetOverride(type);
            if (messageTemplate == null)
            {
                messageTemplate = await GetNewOverride(type);
                await Save(messageTemplate);
            }

            messageTemplate.Bcc = legacyMessageTemplate.Bcc;
            messageTemplate.Body = legacyMessageTemplate.Body;
            messageTemplate.Cc = legacyMessageTemplate.Cc;
            messageTemplate.FromAddress = legacyMessageTemplate.FromAddress;
            messageTemplate.FromName = legacyMessageTemplate.FromName;
            messageTemplate.IsHtml = legacyMessageTemplate.IsHtml;
            messageTemplate.Subject = legacyMessageTemplate.Subject;
            messageTemplate.ToAddress = legacyMessageTemplate.ToAddress;
            messageTemplate.ToName = legacyMessageTemplate.ToName;
            await Save(messageTemplate);
            legacyMessageTemplate.Imported = true;
            await _session.TransactAsync(session => session.SaveAsync(legacyMessageTemplate));
        }

        public async Task<MessageTemplate> GetTemplate(string type)
        {
            var site = _siteLocator.GetCurrentSite();
            var allMessageTemplates = await _messageTemplateProvider.GetAllMessageTemplates(site);
            return
                allMessageTemplates
                    .FirstOrDefault(@base => @base.GetType().FullName == type);
        }

        public async Task Save(MessageTemplate messageTemplate)
        {
            if (messageTemplate.SiteId.HasValue)
                await _messageTemplateProvider.SaveSiteOverride(messageTemplate,
                    await _session.GetAsync<Site>(messageTemplate.SiteId));
            else
                await _messageTemplateProvider.SaveTemplate(messageTemplate);
        }

        private bool CanPreview(MessageTemplate template)
        {
            return template.ModelType != null && typeof(SystemEntity).IsAssignableFrom(template.ModelType);
        }
    }
}