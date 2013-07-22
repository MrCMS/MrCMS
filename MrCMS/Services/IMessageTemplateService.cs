using System.Collections.Generic;
using MrCMS.Entities.Messaging;

namespace MrCMS.Services
{
    public interface IMessageTemplateService
    {
        MessageTemplate Get(int id);
        IList<MessageTemplate> GetAll();
        void Save(MessageTemplate messageTemplate);
    }
}