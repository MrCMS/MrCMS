using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskQueuer
    {
        IList<QueuedTask> GetPendingQueuedTasks();
        IList<QueuedTask> GetPendingLuceneTasks();
        IList<ScheduledTask> GetPendingScheduledTasks();
    }
}