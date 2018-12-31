using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public class QueueMessage : IQueueMessage
    {
        private readonly IEmailSender _emailSender;

        public QueueMessage(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void Queue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null, bool trySendImmediately = true)
        {
            if (queuedMessage != null)
            {
                _emailSender.AddToQueue(queuedMessage, attachments);
                if (trySendImmediately)
                {
                    _emailSender.SendMailMessage(queuedMessage);
                }
            }
        }
    }
}