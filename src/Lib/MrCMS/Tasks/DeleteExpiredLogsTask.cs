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
        private readonly SiteSettings _siteSettings;
        private readonly IRepository<Log> _repository;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public DeleteExpiredLogsTask(SiteSettings siteSettings, IRepository<Log> repository, IGetDateTimeNow getDateTimeNow)
        {
            _siteSettings = siteSettings;
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

        private Task<List<Log>> GetLogs(CancellationToken token)
        {
            var now = _getDateTimeNow.LocalNow;
            return
                _repository.Query()
                    .Where(data => data.CreatedOn <= now.AddDays(-_siteSettings.DaysToKeepLogs))
                    .Take(1000)
                    .ToListAsync(token);
        }
    }
}