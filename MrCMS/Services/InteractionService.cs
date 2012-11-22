using System.Text;
using MrCMS.Entities.Interaction;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.Services
{
    public class InteractionService : IInteractionService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;
        private readonly MailSettings _mailSettings;

        public InteractionService(ISession session, SiteSettings siteSettings, MailSettings mailSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
            _mailSettings = mailSettings;
        }

        public void AddContactUs(ContactUs contactUs)
        {
            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(contactUs);

                                      var queuedMessage =
                                          new QueuedMessage
                                              {
                                                  Subject =
                                                      string.Format("Contact Us form submitted: {0}", contactUs.Subject),
                                                  Body = GetBody(contactUs),
                                                  IsHtml = false
                                              };

                                      SendNotificationMessage(queuedMessage);
                                  });
        }

        public void SendNotificationMessage(QueuedMessage queuedMessage)
        {
            _session.Transact(session =>
                                  {
                                      queuedMessage.FromAddress = _siteSettings.SystemEmailAddress;
                                      queuedMessage.ToAddress = _siteSettings.SystemEmailAddress;

                                      session.SaveOrUpdate(queuedMessage);
                                  });

            TaskExecutor.ExecuteLater(new SendQueuedMessagesTask(_mailSettings));
        }

        private string GetBody(ContactUs contactUs)
        {
            var bodyText = new StringBuilder();

            bodyText.AppendLine(string.Format("Name: {0}", contactUs.Name));
            bodyText.AppendLine(string.Format("Email: {0}", contactUs.Email));
            bodyText.AppendLine(string.Format("Phone: {0}", contactUs.Phone));
            bodyText.AppendLine(string.Format("Subject: {0}", contactUs.Subject));
            bodyText.AppendLine(string.Format("Message: {0}", contactUs.Message));

            return bodyText.ToString();
        }
    }
}