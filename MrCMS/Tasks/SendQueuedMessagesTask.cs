using MrCMS.DbConfiguration;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class SendQueuedMessagesTask : SchedulableTask
    {
        public const int MAX_TRIES = 5;
        private readonly IEmailSender _emailSender;
        private readonly ISession _session;

        public SendQueuedMessagesTask(ISession session, IEmailSender emailSender)
        {
            _session = session;
            _emailSender = emailSender;
        }

        public override int Priority
        {
            get { return 5; }
        }

        protected override void OnExecute()
        {
            using (new SiteFilterDisabler(_session))
            {
                _session.Transact(session =>
                {
                    foreach (
                        var queuedMessage in
                            session.QueryOver<QueuedMessage>().Where(
                                message => message.SentOn == null && message.Tries < MAX_TRIES)
                                .Take(50).List())
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
}