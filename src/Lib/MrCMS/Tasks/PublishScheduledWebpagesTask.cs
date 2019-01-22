using System;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class PublishScheduledWebpagesTask : SchedulableTask
    {
        private readonly ISession _session;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public PublishScheduledWebpagesTask(ISession session, IGetDateTimeNow getDateTimeNow)
        {
            _session = session;
            _getDateTimeNow = getDateTimeNow;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override void OnExecute()
        {
            using (new SiteFilterDisabler(_session))
            {
                var now = _getDateTimeNow.LocalNow;
                var due = _session.QueryOver<Webpage>().Where(x => !x.Published && x.PublishOn <= now).List();
                if (!due.Any())
                    return;
                using (new NotificationDisabler(_session.GetContext()))
                {
                    _session.Transact(session =>
                    {
                        foreach (var webpage in due)
                        {
                            webpage.Published = true;
                            session.Update(webpage);
                        }
                    });
                }
            }
        }
    }
}