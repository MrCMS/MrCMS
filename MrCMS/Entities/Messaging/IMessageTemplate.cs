using System.Collections.Generic;

namespace MrCMS.Entities.Messaging
{
    public interface IMessageTemplate
    {
        List<string> GetTokens();
    }
}