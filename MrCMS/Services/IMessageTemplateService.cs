using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using System;

namespace MrCMS.Services
{
    public interface IMessageTemplateService
    {
        IList<MessageTemplate> GetAll();
        void Save(MessageTemplate messageTemplate);
        Dictionary<Type, int> GetAllMessageTemplateTypesWithDetails();
    }
}