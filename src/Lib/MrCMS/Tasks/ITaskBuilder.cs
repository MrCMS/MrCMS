using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITaskBuilder
    {
        Task<IList<AdHocTask>> GetTasksToExecute(IList<QueuedTask> pendingQueuedTasks);
    }
}