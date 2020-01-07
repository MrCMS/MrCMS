using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Website;


namespace MrCMS.Web.Apps.Admin.Services
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

        public List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails()
        {
            List<MessageTemplate> templates = _messageTemplateProvider.GetAllMessageTemplates(_getSiteId.GetId());
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

        public MessageTemplate GetNewOverride(string type)
        {
            Type typeByName = TypeHelper.GetTypeByName(type);
            MessageTemplate messageTemplateBase = _messageTemplateProvider.GetNewMessageTemplate(typeByName);
            if (messageTemplateBase == null) return null;
            messageTemplateBase.SiteId = _getSiteId.GetId();
            return messageTemplateBase;
        }

        public async Task AddOverride(MessageTemplate messageTemplate)
        {
            var siteId = _getSiteId.GetId();
            await _messageTemplateProvider.SaveSiteOverride(messageTemplate, siteId);
        }

        public MessageTemplate GetOverride(string type)
        {
            MessageTemplate messageTemplateBase = GetTemplate(type);
            if (messageTemplateBase != null && messageTemplateBase.SiteId.HasValue)
                return messageTemplateBase;
            return null;
        }


        public void DeleteOverride(string type)
        {
            var messageTemplate = GetOverride(type);
            if (messageTemplate == null)
                return;
            var siteId = _getSiteId.GetId();
            _messageTemplateProvider.DeleteSiteOverride(messageTemplate, siteId);
        }

        public async Task ImportLegacyTemplate(string type)
        {
            var legacyMessageTemplate = _repository.Query()
                .Where(template => template.MessageTemplateType == type)
                .Take(1)
                .SingleOrDefault();
            if (legacyMessageTemplate == null)
                return;
            var messageTemplate = GetOverride(type);
            if (messageTemplate == null)
            {
                messageTemplate = GetNewOverride(type);
                Save(messageTemplate);
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
            Save(messageTemplate);
            legacyMessageTemplate.Imported = true;
            await _repository.Update(legacyMessageTemplate);
        }

        public MessageTemplate GetTemplate(string type)
        {
            return
                _messageTemplateProvider.GetAllMessageTemplates(_getSiteId.GetId())
                    .FirstOrDefault(@base => @base.GetType().FullName == type);
        }

        public void Save(MessageTemplate messageTemplate)
        {
            if (messageTemplate.SiteId.HasValue)
                _messageTemplateProvider.SaveSiteOverride(messageTemplate, _getSiteId.GetId());
            else
                _messageTemplateProvider.SaveTemplate(messageTemplate);
        }

        private bool CanPreview(MessageTemplate template)
        {
            return template.ModelType != null && typeof(SystemEntity).IsAssignableFrom(template.ModelType);
        }
    }
}