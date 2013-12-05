using MrCMS.Entities.Multisite;
using MrCMS.Logging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class DeleteExpiredLogsTask : BackgroundTask
    {
        private readonly SiteSettings _siteSettings;

        public DeleteExpiredLogsTask(SiteSettings siteSettings)
            : base(siteSettings.Site)
        {
            _siteSettings = siteSettings;
        }

        public override void Execute()
        {
            var statelessSession = MrCMSApplication.Get<ISessionFactory>().OpenStatelessSession();
            var sessionDatas =
                statelessSession.QueryOver<Log>().Where(data => data.CreatedOn <= CurrentRequestData.Now.AddDays(-_siteSettings.DaysToKeepLogs)).List();

            using (var transaction = statelessSession.BeginTransaction())
            {
                foreach (var sessionData in sessionDatas)
                {
                    statelessSession.Delete(sessionData);
                }
                transaction.Commit();
            }
        }
    }
}