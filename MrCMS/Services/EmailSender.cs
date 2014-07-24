using System;
using System.Net;
using System.Net.Mail;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services
{
    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly SmtpClient _smtpClient;

        public EmailSender(ISession session, MailSettings mailSettings, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
            _smtpClient = new SmtpClient(mailSettings.Host, mailSettings.Port)
            {
                EnableSsl = mailSettings.UseSSL,
                Credentials =
                    new NetworkCredential(mailSettings.UserName, mailSettings.Password)
            };
        }

        public bool CanSend(QueuedMessage queuedMessage)
        {
            return !string.IsNullOrEmpty(queuedMessage.ToAddress) && _smtpClient.Credentials != null &&
                   !string.IsNullOrWhiteSpace(_smtpClient.Host) && _siteSettings.SiteIsLive;
        }

        public void SendMailMessage(QueuedMessage queuedMessage)
        {
            try
            {
                var mailMessage = BuildMailMessage(queuedMessage);

                _smtpClient.Send(mailMessage);
                queuedMessage.SentOn = CurrentRequestData.Now;
            }
            catch (Exception exception)
            {
                // TODO: Make this work without HTTP context
                CurrentRequestData.ErrorSignal.Raise(exception);
                queuedMessage.Tries++;
            }
            _session.Transact(session => session.SaveOrUpdate(queuedMessage));
        }

        private static MailMessage BuildMailMessage(QueuedMessage queuedMessage)
        {
            var mailMessage = new MailMessage(new MailAddress(queuedMessage.FromAddress, queuedMessage.FromName),
                new MailAddress(queuedMessage.ToAddress, queuedMessage.ToName))
            {
                Subject = queuedMessage.Subject,
                Body = queuedMessage.Body
            };

            if (!string.IsNullOrWhiteSpace(queuedMessage.Cc))
                mailMessage.CC.Add(queuedMessage.Cc);
            if (!string.IsNullOrWhiteSpace(queuedMessage.Bcc))
                mailMessage.Bcc.Add(queuedMessage.Bcc);

            foreach (QueuedMessageAttachment attachment in queuedMessage.QueuedMessageAttachments)
                mailMessage.Attachments.Add(new Attachment(attachment.FileName));

            mailMessage.IsBodyHtml = queuedMessage.IsHtml;
            return mailMessage;
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }
    }
}