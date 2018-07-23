using System.Collections.Generic;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IMessageTemplateParser
    {
        string Parse<T>(string template, T instance);
        string Parse(string template);
        List<string> GetAllTokens<T>();
        List<string> GetAllTokens(MessageTemplate template);
    }
}