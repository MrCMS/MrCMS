using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Logging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Tasks
{
    public class DeleteExpiredLogsTask : SchedulableTask
    {
        private readonly IStatelessSession _statelessSession;
        private readonly IConfigurationProviderFactory _configurationProviderFactory;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public DeleteExpiredLogsTask(IStatelessSession statelessSession,
            IConfigurationProviderFactory configurationProviderFactory, IGetDateTimeNow getDateTimeNow)
        {
            _statelessSession = statelessSession;
            _configurationProviderFactory = configurationProviderFactory;
            _getDateTimeNow = getDateTimeNow;
        }

        protected override async Task OnExecute()
        {
            foreach (var site in await _statelessSession.Query<Site>().ToListAsync())
            {
                var configProvider = _configurationProviderFactory.GetForSite(site);
                var siteSettings = configProvider.GetSiteSettings<SiteSettings>();
                var now = _getDateTimeNow.LocalNow;
                var expiredLogTime = now.AddDays(-siteSettings.DaysToKeepLogs);

                await _statelessSession.Query<Log>().Where(data =>
                    data.Site.Id == site.Id && data.CreatedOn <= expiredLogTime).DeleteAsync();
            }
        }
    }
}