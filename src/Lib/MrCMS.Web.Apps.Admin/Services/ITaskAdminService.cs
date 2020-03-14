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
        Task<IPagedList<QueuedTask>> GetQueuedTasks(QueuedTaskSearchQuery searchQuery);
        Task Update(TaskUpdateData info);
        Task Reset(TaskUpdateData info);
    }
}