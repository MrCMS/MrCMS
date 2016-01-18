using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Tasks;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ITaskAdminService
    {
        List<TaskInfo> GetAllScheduledTasks();
        TaskUpdateData GetTaskUpdateData(string type);
        IPagedList<QueuedTask> GetQueuedTasks(QueuedTaskSearchQuery searchQuery);
        void Update(TaskUpdateData info);
        void Reset(TaskUpdateData info);
    }
}