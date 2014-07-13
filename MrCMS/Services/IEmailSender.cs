using MrCMS.Entities.Messaging;

namespace MrCMS.Services
{
    public interface IEmailSender
    {
        bool CanSend(QueuedMessage queuedMessage);
        void SendMailMessage(QueuedMessage queuedMessage);
    }
}