using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public interface IEmailSender
    {
        bool CanSend(QueuedMessage queuedMessage);
        void SendMailMessage(QueuedMessage queuedMessage);
        void AddToQueue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null);
    }
}