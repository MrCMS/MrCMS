using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Elmah;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services
{
    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly ErrorSignal _errorSignal;
        private readonly ISession _session;
        private readonly SmtpClient _smtpClient;

        public EmailSender(ISession session, MailSettings mailSettings, ErrorSignal errorSignal)
        {
            _session = session;
            _errorSignal = errorSignal;
            _smtpClient = new SmtpClient(mailSettings.Host, mailSettings.Port)
            {
                EnableSsl = mailSettings.UseSSL,
                Credentials =
                    new NetworkCredential(mailSettings.UserName, mailSettings.Password)
            };
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
                queuedMessage.SentOn = DateTime.Now;
            }
            catch (Exception exception)
            {
                _errorSignal.Raise(exception);
                queuedMessage.Tries++;
            }
            _session.Transact(session => session.SaveOrUpdate(queuedMessage));
        }

        public void AddToQueue(QueuedMessage queuedMessage, List<AttachmentData> attachments = null)
        {
            _session.Transact(session =>
            {
                session.Save(queuedMessage);
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
                    session.Save(attachment);
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
            var multipleToAddress = queuedMessage.ToAddress.Split(new[] {',', ';'},
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