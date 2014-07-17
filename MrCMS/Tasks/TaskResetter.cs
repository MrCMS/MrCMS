using System;
using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

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
                IList<QueuedTask> hungTasks = session.QueryOver<QueuedTask>()
                    .Where(
                        task => task.Site.Id == _site.Id &&
                                (task.Status == TaskExecutionStatus.AwaitingExecution ||
                                 task.Status == TaskExecutionStatus.Executing) &&
                                task.QueuedAt < now.AddMinutes(-15))
                    .List();
                foreach (QueuedTask task in hungTasks)
                {
                    task.QueuedAt = null;
                    task.Status = TaskExecutionStatus.Pending;
                    session.Update(task);
                }
                IList<ScheduledTask> hungScheduledTasks = session.QueryOver<ScheduledTask>()
                    .Where(
                        task => task.Site.Id == _site.Id &&
                                (task.Status == TaskExecutionStatus.AwaitingExecution ||
                                 task.Status == TaskExecutionStatus.Executing) &&
                                (task.LastQueuedAt < now.AddMinutes(-15) || task.LastQueuedAt == null)
                    )
                    .List();
                foreach (ScheduledTask task in hungScheduledTasks)
                {
                    task.Status = TaskExecutionStatus.Pending;
                    session.Update(task);
                }
            });
        }
    }
}