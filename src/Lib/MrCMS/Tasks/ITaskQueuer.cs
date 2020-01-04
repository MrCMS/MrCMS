using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public interface ITaskQueuer
    {
        Task<IList<QueuedTask>> GetPendingQueuedTasks();
        Task<IList<QueuedTask>> GetPendingLuceneTasks();
        Task<IList<Site>> GetPendingQueuedTaskSites();
    }
}