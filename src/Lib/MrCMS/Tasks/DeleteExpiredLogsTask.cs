using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
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
                var logs = await GetLogs(site, siteSettings);

                while (logs.Any())
                {
                    IList<Log> currentLogs = logs;
                    await _statelessSession.TransactAsync(async session =>
                    {
                        foreach (var log in currentLogs)
                        {
                            await session.DeleteAsync(log);
                        }
                    });
                    logs = await GetLogs(site, siteSettings);
                }
            }
        }

        private async Task<IList<Log>> GetLogs(Site site, SiteSettings siteSettings)
        {
            var now = _getDateTimeNow.LocalNow;
            return await
                _statelessSession.QueryOver<Log>()
                    .Where(data =>
                        data.Site.Id == site.Id && data.CreatedOn <= now.AddDays(-siteSettings.DaysToKeepLogs))
                    .Take(1000)
                    .ListAsync();
        }
    }
}