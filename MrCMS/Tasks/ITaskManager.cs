using System.Collections.Generic;
using MrCMS.Paging;

namespace MrCMS.Tasks
{
    public interface ITaskManager
    {
        List<ScheduledTask> GetAllScheduledTasks();
        IPagedList<QueuedTask> GetQueuedTask(QueuedTaskSearchQuery searchQuery);
        void Add(ScheduledTask scheduledTask);
        void Update(ScheduledTask scheduledTask);
        void Delete(ScheduledTask scheduledTask);
    }
}