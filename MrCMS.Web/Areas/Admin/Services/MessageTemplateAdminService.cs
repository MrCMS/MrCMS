using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class MessageTemplateAdminService : IMessageTemplateAdminService
    {
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly ISession _session;
        private readonly Site _site;

        public MessageTemplateAdminService(IMessageTemplateProvider messageTemplateProvider, Site site, ISession session)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _site = site;
            _session = session;
        }

        public List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails()
        {
            List<MessageTemplate> templates = _messageTemplateProvider.GetAllMessageTemplates(_site);
            IList<string> legacyMessageTemplateTypes =
                _session.QueryOver<LegacyMessageTemplate>()
                    .Where(template => !template.Imported)
                    .Select(template => template.MessageTemplateType)
                    .Cacheable()
                    .List<string>();
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
            messageTemplateBase.SiteId = _site.Id;
            return messageTemplateBase;
        }

        public void AddOverride(MessageTemplate messageTemplate)
        {
            _messageTemplateProvider.SaveSiteOverride(messageTemplate, _site);
        }

        public MessageTemplate GetOverride(string type)
        {
            MessageTemplate messageTemplateBase = GetTemplate(type);
            if (messageTemplateBase != null && messageTemplateBase.SiteId.HasValue)
                return messageTemplateBase;
            return null;
        }

        public void DeleteOverride(MessageTemplate messageTemplate)
        {
            _messageTemplateProvider.DeleteSiteOverride(messageTemplate, _site);
        }

        public void ImportLegacyTemplate(string type)
        {
            var legacyMessageTemplate = _session.QueryOver<LegacyMessageTemplate>()
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
            _session.Transact(session => session.Save(legacyMessageTemplate));
        }

        public MessageTemplate GetTemplate(string type)
        {
            return
                _messageTemplateProvider.GetAllMessageTemplates(_site)
                    .FirstOrDefault(@base => @base.GetType().FullName == type);
        }

        public void Save(MessageTemplate messageTemplate)
        {
            _messageTemplateProvider.SaveTemplate(messageTemplate);
        }

        private bool CanPreview(MessageTemplate template)
        {
            return template.ModelType != null && typeof(SystemEntity).IsAssignableFrom(template.ModelType);
        }
    }
}