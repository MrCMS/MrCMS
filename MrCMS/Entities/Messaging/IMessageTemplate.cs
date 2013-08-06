using System.Collections.Generic;

namespace MrCMS.Entities.Messaging
{
    public interface IMessageTemplate<T>
    {
        List<string> GetTokens();
    }
}