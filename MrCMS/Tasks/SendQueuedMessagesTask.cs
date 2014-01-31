using System;
using System.Net;
using System.Net.Mail;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class SendQueuedMessagesTask : SchedulableTask
    {
        public const int MAX_TRIES = 5;
        private readonly MailSettings _mailSettings;
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public SendQueuedMessagesTask(ISession session, MailSettings mailSettings, SiteSettings siteSettings)
        {
            _session = session;
            _mailSettings = mailSettings;
            _siteSettings = siteSettings;
        }

        public override int Priority
        {
            get { return 5; }
        }

        protected override void OnExecute()
        {
                using (var smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
                                            {
                                                EnableSsl = _mailSettings.UseSSL,
                                                Credentials =
                                                    new NetworkCredential(_mailSettings.UserName, _mailSettings.Password)
                                            })
                {
                    _session.Transact(session =>
                                          {
                                              foreach (
                                                  QueuedMessage queuedMessage in
                                                      session.QueryOver<QueuedMessage>().Where(
                                                          message => message.SentOn == null && message.Tries < MAX_TRIES)
                                                             .List())
                                              {
                                                  if (CanSend(queuedMessage, smtpClient))
                                                      SendMailMessage(queuedMessage, smtpClient);
                                                  else
                                                      MarkAsSent(queuedMessage);
                                                  session.SaveOrUpdate(queuedMessage);
                                              }
                                          });
                }
        }

        private bool CanSend(QueuedMessage queuedMessage, SmtpClient smtpClient)
        {
            return !string.IsNullOrEmpty(queuedMessage.ToAddress) && smtpClient.Credentials != null &&
                   !string.IsNullOrWhiteSpace(smtpClient.Host) && _siteSettings.SiteIsLive;
        }

        private void SendMailMessage(QueuedMessage queuedMessage, SmtpClient smtpClient)
        {
            try
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

                smtpClient.Send(mailMessage);
                MarkAsSent(queuedMessage);
            }
            catch (Exception exception)
            {
                // TODO: Make this work without HTTP context
                CurrentRequestData.ErrorSignal.Raise(exception);
                queuedMessage.Tries++;
            }
        }

        private static void MarkAsSent(QueuedMessage queuedMessage)
        {
            queuedMessage.SentOn = CurrentRequestData.Now;
        }
    }
}