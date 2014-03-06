using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskBuilder
    {
        IList<IExecutableTask> GetTasksToExecute(IList<QueuedTask> pendingQueuedTasks, IList<ScheduledTask> pendingScheduledTasks);
    }
}