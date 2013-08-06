using System.Collections.Generic;
using System.Reflection;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using NHibernate;
using System.Linq;
using System;

namespace MrCMS.Services
{
    public class MessageTemplateService : IMessageTemplateService
    {
        private readonly ISession _session;
        private readonly IMessageTemplateParser _messageTemplateParser;

        public MessageTemplateService(ISession session, IMessageTemplateParser messageTemplateParser)
        {
            _session = session;
            _messageTemplateParser = messageTemplateParser;
        }

        public Dictionary<Type, int> GetAllMessageTemplateTypesWithDetails()
        {
            var messageTemplates = new Dictionary<Type, int>();
            var templates = _session.QueryOver<MessageTemplate>().Cacheable().List();
            var messageTemplateTypes = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<MessageTemplate>();
            foreach (var messageTemplateType in messageTemplateTypes)
            {
                var existingMessageTemplate = templates.SingleOrDefault(x => x.GetType() == messageTemplateType);
                messageTemplates.Add(messageTemplateType, existingMessageTemplate != null ? existingMessageTemplate.Id : 0);
            }
            return messageTemplates;
        }

        public MessageTemplate GetNew(string type)
        {
            var newType = TypeHelper.GetTypeByClassName(type);
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

        public void Save(MessageTemplate messageTemplate)
        {
            _session.Transact(session => session.SaveOrUpdate(messageTemplate));
        }
    }
}