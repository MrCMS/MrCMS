using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ITaskAdminService
    {
        Task<List<TaskInfo>> GetAllScheduledTasks();
        Task<TaskUpdateData> GetTaskUpdateData(string type);
        IPagedList<QueuedTask> GetQueuedTasks(QueuedTaskSearchQuery searchQuery);
        void Update(TaskUpdateData info);
        void Reset(TaskUpdateData info);
    }
}