using System.Collections.Generic;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
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

        public IList<QueuedTask> GetPendingQueuedTasks()
        {
            return _session.Transact(session =>
            {
                var queuedAt = CurrentRequestData.Now;
                var queuedTasks =
                    session.QueryOver<QueuedTask>()
                        .Where(task => task.Status == TaskExecutionStatus.Pending)
                        .List();

                foreach (var task in queuedTasks)
                {
                    task.Status = TaskExecutionStatus.AwaitingExecution;
                    task.QueuedAt = queuedAt;
                    _session.Update(task);
                }
                return queuedTasks;
            });
        }


        public IList<Site> GetPendingQueuedTaskSites()
        {
            using (new SiteFilterDisabler(_session))
            {
                Site siteAlias = null;
                return _session.QueryOver(() => siteAlias)
                    .WithSubquery.WhereExists(QueryOver.Of<QueuedTask>()
                            .Where(task => task.Status == TaskExecutionStatus.Pending && task.Site.Id == siteAlias.Id)
                            .Select(task => task.Id))
                            .List();
            }
        }

        public IList<QueuedTask> GetPendingLuceneTasks()
        {
            return _session.Transact(session =>
            {
                var queuedAt = CurrentRequestData.Now;
                var queuedTasks =
                    session.QueryOver<QueuedTask>()
                        .Where(task => (task.Type.IsLike(typeof(InsertIndicesTask<>).FullName, MatchMode.Start) ||
                                        task.Type.IsLike(typeof(UpdateIndicesTask<>).FullName, MatchMode.Start) ||
                                        task.Type.IsLike(typeof(DeleteIndicesTask<>).FullName, MatchMode.Start)
                            ) && task.Status == TaskExecutionStatus.Pending)
                        .List();

                foreach (var task in queuedTasks)
                {
                    task.Status = TaskExecutionStatus.AwaitingExecution;
                    task.QueuedAt = queuedAt;
                    _session.Update(task);
                }
                return queuedTasks;
            });
        }
    }
}