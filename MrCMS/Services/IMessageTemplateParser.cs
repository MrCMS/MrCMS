using System.Collections.Generic;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IMessageTemplateParser
    {
        string Parse<T>(string template, T instance);
        List<string> GetAllTokens<T>();
        List<string> GetAllTokens(MessageTemplateBase template);
    }
}