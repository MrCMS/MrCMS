using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Tasks
{
    public class TaskQueuer : ITaskQueuer
    {
        private readonly ISession _session;

        public TaskQueuer(ISession session)
        {
            _session = session;
        }

        public async Task<IList<QueuedTask>> GetPendingQueuedTasks()
        {
            return await _session.TransactAsync(async session =>
            {
                var queuedAt = DateTime.UtcNow;
                var queuedTasks = await
                    session.QueryOver<QueuedTask>()
                        .Where(task => task.Status == TaskExecutionStatus.Pending)
                        .ListAsync();

                foreach (var task in queuedTasks)
                {
                    task.Status = TaskExecutionStatus.AwaitingExecution;
                    task.QueuedAt = queuedAt;
                    await _session.UpdateAsync(task);
                }
                return queuedTasks;
            });
        }


        public async Task<IList<Site>> GetPendingQueuedTaskSites()
        {
            using (new SiteFilterDisabler(_session))
            {
                Site siteAlias = null;
                return await _session.QueryOver(() => siteAlias)
                    .WithSubquery.WhereExists(QueryOver.Of<QueuedTask>()
                            .Where(task => task.Status == TaskExecutionStatus.Pending && task.Site.Id == siteAlias.Id)
                            .Select(task => task.Id))
                            .ListAsync();
            }
        }
    }
}