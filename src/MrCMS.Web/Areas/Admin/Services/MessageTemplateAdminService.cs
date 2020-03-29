using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class MessageTemplateAdminService : IMessageTemplateAdminService
    {
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly IRepository<LegacyMessageTemplate> _repository;
        private readonly IGetSiteId _getSiteId;

        public MessageTemplateAdminService(IMessageTemplateProvider messageTemplateProvider, IRepository<LegacyMessageTemplate> repository, IGetSiteId getSiteId)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _repository = repository;
            _getSiteId = getSiteId;
        }

        public async Task<List<MessageTemplateInfo>> GetAllMessageTemplateTypesWithDetails()
        {
            List<MessageTemplate> templates = await _messageTemplateProvider.GetAllMessageTemplates(_getSiteId.GetId());
            IList<string> legacyMessageTemplateTypes =
                _repository.Query()
                    .Where(template => !template.Imported)
                    .Select(template => template.MessageTemplateType)
                    .ToList();
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
            messageTemplateBase.SiteId = _getSiteId.GetId();
            return messageTemplateBase;
        }

        public async Task AddOverride(MessageTemplate messageTemplate)
        {
            var siteId = _getSiteId.GetId();
            await _messageTemplateProvider.SaveSiteOverride(messageTemplate, siteId);
        }

        public async Task<MessageTemplate> GetOverride(string type)
        {
            MessageTemplate messageTemplateBase = await GetTemplate(type);
            if (messageTemplateBase?.SiteId != null)
                return messageTemplateBase;
            return null;
        }


        public async Task DeleteOverride(string type)
        {
            var messageTemplate = await GetOverride(type);
            if (messageTemplate == null)
                return;
            var siteId = _getSiteId.GetId();
            await _messageTemplateProvider.DeleteSiteOverride(messageTemplate, siteId);
        }

        public async Task ImportLegacyTemplate(string type)
        {
            var legacyMessageTemplate = _repository.Query()
                .Where(template => template.MessageTemplateType == type)
                .Take(1)
                .SingleOrDefault();
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
            await _repository.Update(legacyMessageTemplate);
        }

        public async Task<MessageTemplate> GetTemplate(string type)
        {
            var allMessageTemplates = await _messageTemplateProvider.GetAllMessageTemplates(_getSiteId.GetId());
            return
                allMessageTemplates
                    .FirstOrDefault(@base => @base.GetType().FullName == type);
        }

        public async Task Save(MessageTemplate messageTemplate)
        {
            if (messageTemplate.SiteId.HasValue)
                await _messageTemplateProvider.SaveSiteOverride(messageTemplate, _getSiteId.GetId());
            else
                await _messageTemplateProvider.SaveTemplate(messageTemplate);
        }

        private bool CanPreview(MessageTemplate template)
        {
            return template.ModelType != null && typeof(SystemEntity).IsAssignableFrom(template.ModelType);
        }
    }
}