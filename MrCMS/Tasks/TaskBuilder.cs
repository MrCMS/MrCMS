using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Tasks
{
    public class TaskBuilder : ITaskBuilder
    {
        private readonly IKernel _kernel;
        private readonly ISession _session;

        public TaskBuilder(IKernel kernel,ISession session)
        {
            _kernel = kernel;
            _session = session;
        }

        public IList<IExecutableTask> GetTasksToExecute(IList<QueuedTask> pendingQueuedTasks)
        {
            var executableTasks = new List<IExecutableTask>();
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
                    CurrentRequestData.ErrorSignal.Raise(exception);
                    failedTasks.Add(queuedTask);
                }
            }
            if (failedTasks.Any())
            {
                _session.Transact(session => failedTasks.ForEach(task =>
                {
                    task.Status = TaskExecutionStatus.Failed;
                    task.FailedAt = CurrentRequestData.Now;
                    session.Update(task);
                }));
            }
            return executableTasks;
        }

        private IExecutableTask GetTask(ScheduledTask scheduledTask)
        {
            var taskType = TypeHelper.GetAllTypes().FirstOrDefault(type => type.FullName == scheduledTask.Type);
            var task = _kernel.Get(taskType) as IExecutableTask;
            task.Site = scheduledTask.Site;
            task.Entity = scheduledTask;
            return task;
        }

        private IExecutableTask GetTask(QueuedTask queuedTask)
        {
            var task = _kernel.Get(queuedTask.GetTaskType()) as IExecutableTask;
            task.Site = queuedTask.Site;
            task.Entity = queuedTask;
            task.SetData(queuedTask.Data);
            return task;
        }

        public IExecutableTask GetTask(Guid id)
        {
            var scheduledTask = _session.GetByGuid<ScheduledTask>(id);
            if (scheduledTask != null)
                return GetTask(scheduledTask);
            var queuedTask = _session.GetByGuid<QueuedTask>(id);
            return queuedTask != null
                ? GetTask(queuedTask)
                : null;
        }
    }
}