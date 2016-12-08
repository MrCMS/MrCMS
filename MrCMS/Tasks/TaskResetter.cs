using System;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks.Entities;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskResetter : ITaskResetter
    {
        private readonly ISession _session;
        private readonly ITaskSettingManager _taskSettingManager;

        public TaskResetter(ISession session, ITaskSettingManager taskSettingManager)
        {
            _session = session;
            _taskSettingManager = taskSettingManager;
        }

        public void ResetHungTasks()
        {
            _session.Transact(session =>
            {
                var now = CurrentRequestData.Now;
                using (new SiteFilterDisabler(session))
                    ResetQueuedTasks(session, now);
            });
            ResetScheduledTasks();
        }

        private void ResetScheduledTasks()
        {
            var now = CurrentRequestData.Now;
            var hungScheduledTasks = _taskSettingManager.GetInfo()
                .Where(
                    task => task.Enabled &&
                        (task.Status == TaskExecutionStatus.AwaitingExecution ||
                         task.Status == TaskExecutionStatus.Executing || task.Status == TaskExecutionStatus.Failed) &&
                        (task.LastStarted < now.AddMinutes(-15) || task.LastStarted == null)
                )
                .ToList();
            foreach (var task in hungScheduledTasks)
            {
                _taskSettingManager.Reset(task.Type, false);
            }
        }

        private void ResetQueuedTasks(ISession session, DateTime now)
        {
            var hungTasks = session.QueryOver<QueuedTask>()
                .Where(
                    task =>
                        (task.Status == TaskExecutionStatus.AwaitingExecution ||
                         task.Status == TaskExecutionStatus.Executing) &&
                        task.QueuedAt < now.AddMinutes(-15))
                .List();
            foreach (var task in hungTasks)
            {
                task.QueuedAt = null;
                task.Status = TaskExecutionStatus.Pending;
                session.Update(task);
            }
        }
    }
}