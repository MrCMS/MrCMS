using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskResetter : ITaskResetter
    {
        private readonly ISession _session;
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly ILogger<TaskResetter> _logger;

        public TaskResetter(ISession session, IGetDateTimeNow getDateTimeNow, ILogger<TaskResetter> logger)
        {
            _session = session;
            _getDateTimeNow = getDateTimeNow;
            _logger = logger;
        }

        public async Task ResetHungTasks()
        {
            await _session.TransactAsync(async session =>
            {
                var now = _getDateTimeNow.LocalNow;
                using (new SiteFilterDisabler(session))
                    await ResetQueuedTasks(session, now);
            });
        }

        private async Task ResetQueuedTasks(ISession session, DateTime now)
        {
            var hungTasks = await session.QueryOver<QueuedTask>()
                .Where(
                    task =>
                        (task.Status == TaskExecutionStatus.AwaitingExecution ||
                         task.Status == TaskExecutionStatus.Executing) &&
                        task.QueuedAt < now.AddMinutes(-15))
                .ListAsync();
            if (!hungTasks.Any())
                return;
            
            _logger.LogInformation($"Resetting {hungTasks.Count} task(s)");
            foreach (var task in hungTasks)
            {
                task.QueuedAt = null;
                task.Status = TaskExecutionStatus.Pending;
                await session.UpdateAsync(task);
            }
        }
    }
}