using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface IGetPendingScheduledTasks
    {
        List<TaskInfo> GetTasks();
    }
}