using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Tasks
{
    public class TaskBuilder : ITaskBuilder
    {
        private readonly IKernel _kernel;

        public TaskBuilder(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IList<IExecutableTask> GetTasksToExecute(IList<QueuedTask> pendingQueuedTasks, IList<ScheduledTask> pendingScheduledTasks)
        {
            var executableTasks = new List<IExecutableTask>();
            foreach (var queuedTask in pendingQueuedTasks)
            {
                var task = _kernel.Get(queuedTask.GetTaskType()) as IExecutableTask;
                task.Site = queuedTask.Site;
                task.Entity = queuedTask;
                task.SetData(queuedTask.Data);
                executableTasks.Add(task);
            }
            foreach (var scheduledTask in pendingScheduledTasks)
            {
                var taskType = TypeHelper.GetAllTypes().FirstOrDefault(type => type.FullName == scheduledTask.Type);
                var task = _kernel.Get(taskType) as IExecutableTask;
                task.Site = scheduledTask.Site;
                task.Entity = scheduledTask;
                executableTasks.Add(task);
            }
            return executableTasks;
        }
    }
}