using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskQueuer : ITaskQueuer
    {
        private readonly ISession _session;
        private readonly Site _site;

        public TaskQueuer(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public IList<QueuedTask> GetPendingQueuedTasks()
        {
            return _session.Transact(session =>
                                         {
                                             DateTime queuedAt = CurrentRequestData.Now;
                                             var queuedTasks =
                                                 session.QueryOver<QueuedTask>()
                                                        .Where(task => task.Status == TaskExecutionStatus.Pending && task.Site.Id == _site.Id)
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

        public IList<ScheduledTask> GetPendingScheduledTasks()
        {
            return _session.Transact(session =>
                                         {
                                             DateTime lastQueued = CurrentRequestData.Now;
                                             var scheduledTasks =
                                                 _session.QueryOver<ScheduledTask>().List()
                                                         .Where(task =>
                                                             task.Status == TaskExecutionStatus.Pending && task.Site.Id == _site.Id &&
                                                             (task.LastComplete < lastQueued.AddSeconds(-task.EveryXSeconds) ||
                                                              task.LastComplete == null))
                                                         .ToList();
                                             foreach (var task in scheduledTasks)
                                             {
                                                 task.Status = TaskExecutionStatus.AwaitingExecution;
                                                 task.LastQueuedAt = lastQueued;
                                                 _session.Update(task);
                                             }
                                             return scheduledTasks;
                                         });
        }
    }
}