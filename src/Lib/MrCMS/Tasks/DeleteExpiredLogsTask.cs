using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class DeleteExpiredLogsTask : SchedulableTask
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IRepository<Log> _repository;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public DeleteExpiredLogsTask(IConfigurationProvider configurationProvider, IRepository<Log> repository, IGetDateTimeNow getDateTimeNow)
        {
            _configurationProvider = configurationProvider;
            _repository = repository;
            _getDateTimeNow = getDateTimeNow;
        }

        public override int Priority { get { return 0; } }

        protected override async Task OnExecute(CancellationToken token)
        {
            var logs = await GetLogs(token);

            while (logs.Any())
            {
                IList<Log> currentLogs = logs;
                await _repository.DeleteRange(currentLogs, token);
                logs = await GetLogs(token);
            }
        }

        private async Task<List<Log>> GetLogs(CancellationToken token)
        {
            var now = await _getDateTimeNow.GetLocalNow();
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            return await
                _repository.Query()
                    .Where(data => data.CreatedOn <= now.AddDays(-siteSettings.DaysToKeepLogs))
                    .Take(1000)
                    .ToListAsync(token);
        }
    }
}