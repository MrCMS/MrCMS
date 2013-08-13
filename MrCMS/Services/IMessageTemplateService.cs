using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using System;

namespace MrCMS.Services
{
    public interface IMessageTemplateService
    {
        void Save(MessageTemplate messageTemplate);
        Dictionary<Type, int> GetAllMessageTemplateTypesWithDetails();
        MessageTemplate GetNew(string type);
        MessageTemplate Reset(MessageTemplate messageTemplate);
        List<string> GetTokens(MessageTemplate messageTemplate);
        T Get<T>() where T : MessageTemplate;
    }
}