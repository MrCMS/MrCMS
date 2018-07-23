using System;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Tasks
{
    public class PublishScheduledWebpagesTask : SchedulableTask
    {
        private readonly ISession _session;

        public PublishScheduledWebpagesTask(ISession session)
        {
            _session = session;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override void OnExecute()
        {
            using (new SiteFilterDisabler(_session))
            {
                var now = DateTime.UtcNow;
                var due = _session.QueryOver<Webpage>().Where(x => !x.Published && x.PublishOn <= now).List();
                if (!due.Any())
                    return;
                //using (new NotificationDisabler())
                // TODO: notifications  
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