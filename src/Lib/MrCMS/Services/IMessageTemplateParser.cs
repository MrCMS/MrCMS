using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IMessageTemplateParser
    {
        Task<string> Parse<T>(string template, T instance);
        Task<string> Parse(string template);
        List<string> GetAllTokens<T>();
        List<string> GetAllTokens(MessageTemplate template);
    }
}