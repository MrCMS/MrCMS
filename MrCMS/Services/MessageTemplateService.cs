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
        public MessageTemplate Get(int id)
        {
            return _session.QueryOver<MessageTemplate>().Where(x => x.Id==id).Cacheable().SingleOrDefault();
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
        public void Save(MessageTemplate messageTemplate)
        {
            _session.Transact(session => session.SaveOrUpdate(messageTemplate));
        }
    }
}