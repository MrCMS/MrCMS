using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class SendQueuedMessagesTask : SchedulableTask
    {
        public const int MAX_TRIES = 5;
        private readonly ISession _session;
        private readonly IEmailSender _emailSender;
        private readonly SiteSettings _siteSettings;
        private readonly Site _site;

        public SendQueuedMessagesTask(ISession session, IEmailSender emailSender, SiteSettings siteSettings,Site site)
        {
            _session = session;
            _emailSender = emailSender;
            _siteSettings = siteSettings;
            _site = site;
        }

        public override int Priority
        {
            get { return 5; }
        }

        protected override void OnExecute()
        {
            _session.Transact(session =>
            {
                foreach (
                    QueuedMessage queuedMessage in
                        session.QueryOver<QueuedMessage>().Where(
                            message => message.SentOn == null && message.Tries < MAX_TRIES)
                            .Where(message => message.Site.Id == _site.Id)
                               .List())
                {
                    if (_emailSender.CanSend(queuedMessage))
                        _emailSender.SendMailMessage(queuedMessage);
                    else
                        queuedMessage.SentOn = CurrentRequestData.Now;
                    session.SaveOrUpdate(queuedMessage);
                }
            });
        }
    }
}