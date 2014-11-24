using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMessageTemplateParser _messageTemplateParser;

        public MessageTemplateAdminService(IMessageTemplateProvider messageTemplateProvider, Site site, IMessageTemplateParser messageTemplateParser)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _site = site;
            _messageTemplateParser = messageTemplateParser;
        }

        public List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails()
        {
            var templates = _messageTemplateProvider.GetAllMessageTemplates(_site);
            return templates.Select(template => new MessageTemplateInfo
            {
                Type = template.GetType(),
                IsOverride = template.SiteId.HasValue,
                CanPreview = false
            }).ToList();
        }

        public MessageTemplateBase GetNewOverride(string type)
        {
            var typeByName = TypeHelper.GetTypeByName(type);
            try
            {
                var messageTemplateBase = Activator.CreateInstance(typeByName) as MessageTemplateBase;
                messageTemplateBase.SiteId = _site.Id;
                return messageTemplateBase;
            }
            finally
            {
            }
            return null;

        }

        public void AddOverride(MessageTemplateBase messageTemplate)
        {
            _messageTemplateProvider.SaveSiteOverride(messageTemplate, _site);
        }

        public MessageTemplateBase Reset(MessageTemplateBase messageTemplate)
        {
            var initialTemplate = Activator.CreateInstance(messageTemplate.GetType()) as MessageTemplateBase;

            messageTemplate.FromAddress = initialTemplate.FromAddress;
            messageTemplate.FromName = initialTemplate.FromName;
            messageTemplate.ToAddress = initialTemplate.ToAddress;
            messageTemplate.ToName = initialTemplate.ToName;
            messageTemplate.Bcc = initialTemplate.Bcc;
            messageTemplate.Cc = initialTemplate.Cc;
            messageTemplate.Subject = initialTemplate.Subject;
            messageTemplate.Body = initialTemplate.Body;
            messageTemplate.IsHtml = initialTemplate.IsHtml;

            Save(messageTemplate);

            return messageTemplate;
        }

        public List<string> GetTokens(MessageTemplateBase messageTemplate)
        {
            return _messageTemplateParser.GetAllTokens(messageTemplate);
        }

        public List<string> GetTokens(MessageTemplate messageTemplate)
        {
            return messageTemplate.GetTokens(_messageTemplateParser);
        }

        public T Get<T>() where T : MessageTemplateBase, new()
        {
            return _messageTemplateProvider.GetMessageTemplate<T>(_site);
        }

        public string GetPreview(MessageTemplateBase messageTemplate, int itemId)
        {
            return string.Empty;
        }

        public MessageTemplateBase GetOverride(string type)
        {
            var messageTemplateBase = _messageTemplateProvider.GetAllMessageTemplates(_site).FirstOrDefault(@base => @base.GetType().FullName == type);
            if (messageTemplateBase != null && messageTemplateBase.SiteId.HasValue)
                return messageTemplateBase;
            return null;

        }

        public void DeleteOverride(MessageTemplateBase messageTemplate)
        {
            _messageTemplateProvider.DeleteSiteOverride(messageTemplate, _site);
        }

        public MessageTemplateBase GetTemplate(string type)
        {
            return _messageTemplateProvider.GetAllMessageTemplates(_site).FirstOrDefault(@base => @base.GetType().FullName == type);
        }

        public void Save(MessageTemplateBase messageTemplate)
        {
            _messageTemplateProvider.SaveTemplate(messageTemplate);
        }
    }
}