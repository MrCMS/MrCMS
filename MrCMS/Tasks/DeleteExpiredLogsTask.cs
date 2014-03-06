using MrCMS.Entities.Multisite;
using MrCMS.Logging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class DeleteExpiredLogsTask : SchedulableTask
    {
        private readonly SiteSettings _siteSettings;
        private readonly ISessionFactory _sessionFactory;

        public DeleteExpiredLogsTask(SiteSettings siteSettings, ISessionFactory sessionFactory)
        {
            _siteSettings = siteSettings;
            _sessionFactory = sessionFactory;
        }

        public override int Priority { get { return 0; } }

        protected override void OnExecute()
        {
            var statelessSession = _sessionFactory.OpenStatelessSession();
            var logs =
                statelessSession.QueryOver<Log>().Where(data => data.CreatedOn <= CurrentRequestData.Now.AddDays(-_siteSettings.DaysToKeepLogs)).List();

            using (var transaction = statelessSession.BeginTransaction())
            {
                foreach (var log in logs)
                {
                    statelessSession.Delete(log);
                }
                transaction.Commit();
            }
        }
    }
}