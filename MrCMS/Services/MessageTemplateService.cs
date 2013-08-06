using System.Collections.Generic;
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

        public MessageTemplateService(ISession session)
        {
            _session = session;
        }
        public IList<MessageTemplate> GetAll()
        {
            return _session.QueryOver<MessageTemplate>().Cacheable().List();
        }
        public Dictionary<Type, int> GetAllMessageTemplateTypesWithDetails()
        {
            var messageTemplates = new Dictionary<Type, int>();
            var exisitingMessageTemplates = GetAll();
            var messageTemplateTypes = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<MessageTemplate>();
            foreach (var messageTemplateType in messageTemplateTypes)
            {
                var existingMessageTemplate = exisitingMessageTemplates.SingleOrDefault(x => x.GetType().Name == messageTemplateType.Name);
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
                    return messageTemplate.GetInitialTemplate();
                }
            }
            return null;
        }

        public MessageTemplate Reset(MessageTemplate messageTemplate)
        {
            var initialTemplate = messageTemplate.GetInitialTemplate();

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

        public void Save(MessageTemplate messageTemplate)
        {
            _session.Transact(session => session.SaveOrUpdate(messageTemplate));
        }
    }
}