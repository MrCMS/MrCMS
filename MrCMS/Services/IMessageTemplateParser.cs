using System.Collections.Generic;

namespace MrCMS.Services
{
    public interface IMessageTemplateParser
    {
        string Parse<T>(string template, T instance);
        List<string> GetAllTokens<T>();
    }
}