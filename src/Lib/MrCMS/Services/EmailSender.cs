using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Data;
using MrCMS.Entities.Messaging;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class EmailSender : IEmailSender, IDisposable
    {
        //private readonly ErrorSignal _errorSignal;
        private readonly IRepository<QueuedMessage> _messageRepository;
        private readonly IRepository<QueuedMessageAttachment> _attachmentRepository;
        private readonly ILogger<EmailSender> _logger;
        private readonly SmtpClient _smtpClient;

        public EmailSender(IRepository<QueuedMessage> messageRepository,
            IRepository<QueuedMessageAttachment> attachmentRepository, MailSettings mailSettings,
            ILogger<EmailSender> logger, IGetSmtpClient getSmtpClient)
        {
            _messageRepository = messageRepository;
            _attachmentRepository = attachmentRepository;
            _logger = logger;
            _smtpClient = getSmtpClient.GetClient(mailSettings);
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }

        public bool CanSend(QueuedMessage queuedMessage)
        {
            return !string.IsNullOrEmpty(queuedMessage.ToAddress) && _smtpClient.Credentials != null &&
                   !string.IsNullOrWhiteSpace(_smtpClient.Host);
        }

        public void SendMailMessage(QueuedMessage queuedMessage)
        {
            try
            {
                var mailMessage = BuildMailMessage(queuedMessage);

                _smtpClient.Send(mailMessage);
                queuedMessage.SentOn = DateTime.UtcNow;
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                queuedMessage.Tries++;
            }
            _messageRepository.Update(queuedMessage);
        }

        public async Task AddToQueue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null)
        {
            await _messageRepository.Transact(async (repo, ct) =>
            {
                await repo.Add(queuedMessage, ct);
                if (attachments == null || !attachments.Any())
                    return;
                foreach (var attachment in attachments.Select(data => new QueuedMessageAttachment
                {
                    QueuedMessageId = queuedMessage.Id,
                    ContentType = data.ContentType,
                    FileName = data.FileName,
                    Data = data.Data,
                    FileSize = data.Data.LongLength
                }))
                {
                    queuedMessage.QueuedMessageAttachments.Add(attachment);
                    await _attachmentRepository.Add(attachment, ct);
                }
            });
        }

        private MailMessage BuildMailMessage(QueuedMessage queuedMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(queuedMessage.FromAddress, queuedMessage.FromName),
                Subject = queuedMessage.Subject,
                Body = queuedMessage.Body
            };
            var multipleToAddress = queuedMessage.ToAddress.Split(new[] { ',', ';' },
                StringSplitOptions.RemoveEmptyEntries);
            if (multipleToAddress.Any())
            {
                foreach (var email in multipleToAddress)
                {
                    mailMessage.To.Add(new MailAddress(email.Trim(), queuedMessage.ToName));
                }
            }

            if (!string.IsNullOrWhiteSpace(queuedMessage.Cc))
                mailMessage.CC.Add(queuedMessage.Cc);
            if (!string.IsNullOrWhiteSpace(queuedMessage.Bcc))
                mailMessage.Bcc.Add(queuedMessage.Bcc);

            if (queuedMessage.QueuedMessageAttachments != null)
                foreach (var attachment in queuedMessage.QueuedMessageAttachments)
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.Data), attachment.FileName,
                        attachment.ContentType));

            mailMessage.IsBodyHtml = queuedMessage.IsHtml;
            return mailMessage;
        }
    }
}