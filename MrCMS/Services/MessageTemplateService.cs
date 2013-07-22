using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using NHibernate;

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
        public void Save(MessageTemplate messageTemplate)
        {
            _session.Transact(session => session.SaveOrUpdate(messageTemplate));
        }    
    }
}