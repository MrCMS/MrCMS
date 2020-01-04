using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IEmailSender
    {
        bool CanSend(QueuedMessage queuedMessage);
        void SendMailMessage(QueuedMessage queuedMessage);
        Task AddToQueue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null);
    }
}