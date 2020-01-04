using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task Queue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null, bool trySendImmediately = true)
        {
            if (queuedMessage != null)
            {
                await _emailSender.AddToQueue(queuedMessage, attachments);
                if (trySendImmediately)
                {
                    _emailSender.SendMailMessage(queuedMessage);
                }
            }
        }
    }
}