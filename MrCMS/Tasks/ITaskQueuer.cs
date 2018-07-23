using System.Collections.Generic;
using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public interface ITaskQueuer
    {
        IList<QueuedTask> GetPendingQueuedTasks();
        IList<QueuedTask> GetPendingLuceneTasks();
        IList<Site> GetPendingQueuedTaskSites();
    }
}