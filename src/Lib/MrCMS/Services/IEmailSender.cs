using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IEmailSender
    {
        Task<bool> CanSend(QueuedMessage queuedMessage);
        Task SendMailMessage(QueuedMessage queuedMessage);
        Task AddToQueue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null);
    }
}