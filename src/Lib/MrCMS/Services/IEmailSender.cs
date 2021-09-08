using System.Threading.Tasks;
using MrCMS.Entities.Messaging;

namespace MrCMS.Services
{
    public interface IEmailSender
    {
        bool CanSend(QueuedMessage queuedMessage);
        Task<QueuedMessage> SendMailMessage(QueuedMessage queuedMessage);
    }
}