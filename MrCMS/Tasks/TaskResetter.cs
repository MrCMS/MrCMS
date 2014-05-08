using System;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Tasks
{
    public class TaskResetter : ITaskResetter
    {
        private readonly ISession _session;
        private readonly Site _site;

        public TaskResetter(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public void ResetHungTasks()
        {
            _session.Transact(session =>
                {
                    DateTime now = CurrentRequestData.Now;
                    var hungTasks = session.QueryOver<QueuedTask>()
                                           .Where(
                                               task => task.Site.Id == _site.Id &&
                                                       (task.Status == TaskExecutionStatus.AwaitingExecution || task.Status == TaskExecutionStatus.Executing) &&
                                                       task.QueuedAt < now.AddMinutes(-15))
                                           .List();
                    foreach (var task in hungTasks)
                    {
                        task.QueuedAt = null;
                        task.Status = TaskExecutionStatus.Pending;
                        session.Update(task);
                    }
                    var hungScheduledTasks = session.QueryOver<ScheduledTask>()
                                                    .Where(
                                                        task => task.Site.Id == _site.Id &&
                                                                (task.Status == TaskExecutionStatus.AwaitingExecution || task.Status == TaskExecutionStatus.Executing) &&
                                                                (task.LastQueuedAt < now.AddMinutes(-15) || task.LastQueuedAt == null)
                        )
                                                    .List();
                    foreach (var task in hungScheduledTasks)
                    {
                        task.Status = TaskExecutionStatus.Pending;
                        session.Update(task);
                    }
                });
        }
    }
}