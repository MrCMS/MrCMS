using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Tasks;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface ITaskAdminService
    {
        Task<List<TaskInfo>> GetAllScheduledTasks();
        Task<TaskUpdateData> GetTaskUpdateData(string type);
        Task<IPagedList<QueuedTask>> GetQueuedTasks(QueuedTaskSearchQuery searchQuery);
        Task<bool> Update(TaskUpdateData info);
        Task Execute(TaskUpdateData taskInfo);
    }
}