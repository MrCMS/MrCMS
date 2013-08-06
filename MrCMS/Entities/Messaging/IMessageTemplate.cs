using System.Collections.Generic;
using MrCMS.Services;

namespace MrCMS.Entities.Messaging
{
    public interface IMessageTemplate<T>
    {
        List<string> GetTokens(IMessageTemplateParser messageTemplateParser);
    }
}