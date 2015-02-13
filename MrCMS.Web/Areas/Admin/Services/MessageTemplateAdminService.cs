using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class MessageTemplateAdminService : IMessageTemplateAdminService
    {
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly Site _site;

        public MessageTemplateAdminService(IMessageTemplateProvider messageTemplateProvider, Site site)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _site = site;
        }

        public List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails()
        {
            var templates = _messageTemplateProvider.GetAllMessageTemplates(_site);
            return templates.Select(template => new MessageTemplateInfo
            {
                Type = template.GetType(),
                IsOverride = template.SiteId.HasValue,
                CanPreview = CanPreview(template),
                IsEnabled = !template.IsDisabled
            }).ToList();
        }

        private bool CanPreview(MessageTemplate template)
        {
            return template.ModelType != null && typeof(SystemEntity).IsAssignableFrom(template.ModelType);
        }

        public MessageTemplate GetNewOverride(string type)
        {
            var typeByName = TypeHelper.GetTypeByName(type);
            var messageTemplateBase = _messageTemplateProvider.GetNewMessageTemplate(typeByName);
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
            var messageTemplateBase = GetTemplate(type);
            if (messageTemplateBase != null && messageTemplateBase.SiteId.HasValue)
                return messageTemplateBase;
            return null;

        }

        public void DeleteOverride(MessageTemplate messageTemplate)
        {
            _messageTemplateProvider.DeleteSiteOverride(messageTemplate, _site);
        }

        public MessageTemplate GetTemplate(string type)
        {
            return _messageTemplateProvider.GetAllMessageTemplates(_site).FirstOrDefault(@base => @base.GetType().FullName == type);
        }

        public void Save(MessageTemplate messageTemplate)
        {
            _messageTemplateProvider.SaveTemplate(messageTemplate);
        }
    }
}