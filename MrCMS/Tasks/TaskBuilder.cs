using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskBuilder : ITaskBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISession _session;

        public TaskBuilder(IServiceProvider serviceProvider,ISession session)
        {
            _serviceProvider = serviceProvider;
            _session = session;
        }

        public IList<AdHocTask> GetTasksToExecute(IList<QueuedTask> pendingQueuedTasks)
        {
            var executableTasks = new List<AdHocTask>();
            var failedTasks = new List<QueuedTask>();
            foreach (var queuedTask in pendingQueuedTasks)
            {
                try
                {
                    var task = GetTask(queuedTask);
                    executableTasks.Add(task);
                }
                catch (Exception exception)
                {
                    //CurrentRequestData.ErrorSignal.Raise(exception);
                    // TODO: add logging
                    failedTasks.Add(queuedTask);
                }
            }
            if (failedTasks.Any())
            {
                _session.Transact(session => failedTasks.ForEach(task =>
                {
                    task.Status = TaskExecutionStatus.Failed;
                    task.FailedAt = DateTime.UtcNow;
                    session.Update(task);
                }));
            }
            return executableTasks;
        }

        private AdHocTask GetTask(QueuedTask queuedTask)
        {
            var task = _serviceProvider.GetService(queuedTask.GetTaskType()) as AdHocTask;
            task.Site = queuedTask.Site;
            task.Entity = queuedTask;
            task.SetData(queuedTask.Data);
            return task;
        }

    }
}