using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Messages;
using NHibernate;

namespace MrCMS.Services
{
    public class QueueMessage : IQueueMessage
    {
        private readonly ISession _session;
        private readonly IEmailSender _emailSender;

        public QueueMessage(ISession session, IEmailSender emailSender)
        {
            _session = session;
            _emailSender = emailSender;
        }

        public async Task Queue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null,
            bool trySendImmediately = true)
        {
            if (queuedMessage != null)
            {
                await AddToQueue(queuedMessage, attachments);
                if (trySendImmediately)
                {
                    var sendMailMessage = await _emailSender.SendMailMessage(queuedMessage);
                    await Update(queuedMessage: sendMailMessage);
                }
            }
        }

        private async Task Update(QueuedMessage queuedMessage)
        {
            await _session.TransactAsync(session => session.UpdateAsync(queuedMessage));
        }

        private async Task AddToQueue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null)
        {
            await _session.TransactAsync(async session =>
            {
                await session.SaveAsync(queuedMessage);
                if (attachments == null || !attachments.Any())
                    return;
                foreach (var data in attachments)
                {
                    var attachment = new QueuedMessageAttachment
                    {
                        QueuedMessage = queuedMessage,
                        ContentType = data.ContentType,
                        FileName = data.FileName,
                        Data = data.Data,
                        FileSize = data.Data.LongLength
                    };
                    queuedMessage.QueuedMessageAttachments.Add(attachment);
                    await session.SaveAsync(attachment);
                }
            });
        }
    }
}