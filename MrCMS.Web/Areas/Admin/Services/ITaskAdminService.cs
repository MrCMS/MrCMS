using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Tasks;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ITaskAdminService
    {
        List<ScheduledTask> GetAllScheduledTasks();
        IPagedList<QueuedTask> GetQueuedTasks(QueuedTaskSearchQuery searchQuery);
        void Add(ScheduledTask scheduledTask);
        void Update(ScheduledTask scheduledTask);
        void Delete(ScheduledTask scheduledTask);
    }
}