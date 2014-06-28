using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class MessageTemplateAdminService : IMessageTemplateAdminService
    {
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IMessageTemplateParser _messageTemplateParser;

        public MessageTemplateAdminService(ISession session, Site site, IMessageTemplateParser messageTemplateParser)
        {
            _session = session;
            _site = site;
            _messageTemplateParser = messageTemplateParser;
        }

        public List<MessageTemplateInfo> GetAllMessageTemplateTypesWithDetails()
        {
            var templates =
                _session.QueryOver<MessageTemplate>().Where(template => template.Site == _site).Cacheable().List();
            var messageTemplateTypes = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<MessageTemplate>();
            return messageTemplateTypes.Select(type =>
            {

                var existingMessageTemplate =
                    templates.SingleOrDefault(x => x.GetType() == type);
                return new MessageTemplateInfo
                {
                    Type = type,
                    Id =
                        existingMessageTemplate != null
                            ? existingMessageTemplate.Id
                            : (int?)null,
                    CanPreview =
                        existingMessageTemplate != null &&
                        existingMessageTemplate.CanPreview
                };
            }).ToList();
        }

        public MessageTemplate GetNew(string type)
        {
            var newType = TypeHelper.GetTypeByName(type);
            if (newType != null)
            {
                var messageTemplate = Activator.CreateInstance(newType) as MessageTemplate;
                if (messageTemplate != null)
                {
                    return messageTemplate.GetInitialTemplate(_session);
                }
            }
            return null;
        }

        public MessageTemplate Reset(MessageTemplate messageTemplate)
        {
            var initialTemplate = messageTemplate.GetInitialTemplate(_session);

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

        public List<string> GetTokens(MessageTemplate messageTemplate)
        {
            return messageTemplate.GetTokens(_messageTemplateParser);
        }

        public T Get<T>() where T : MessageTemplate
        {
            return _session.QueryOver<T>().Where(arg => arg.Site == _site).Take(1).Cacheable().SingleOrDefault();
        }

        public string GetPreview(MessageTemplate messageTemplate, int itemId)
        {
            var parse = _messageTemplateParser.GetType().GetMethod("Parse");
            var parseGeneric = parse.MakeGenericMethod(messageTemplate.PreviewType);
            return parseGeneric.Invoke(_messageTemplateParser, new object[]
            {
                messageTemplate.Body,
                _session.Get(messageTemplate.PreviewType, itemId)
            }) as string;
        }

        public void Save(MessageTemplate messageTemplate)
        {
            _session.Transact(session => session.SaveOrUpdate(messageTemplate));
        }
    }
}