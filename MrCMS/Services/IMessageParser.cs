using MrCMS.Entities.Messaging;

namespace MrCMS.Services
{
    public interface IMessageParser<T, in T2> where T : MessageTemplate, IMessageTemplate<T2>
    {
        QueuedMessage GetMessage(T2 obj, string fromAddress = null, string fromName = null, string toAddress = null, string toName = null,
                                 string cc = null, string bcc = null);
    }
}