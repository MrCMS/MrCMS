using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface IGetPendingScheduledTasks
    {
        Task<List<TaskInfo>> GetTasks();
    }
}