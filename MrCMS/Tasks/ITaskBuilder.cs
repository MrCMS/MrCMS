using System;
using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskBuilder
    {
        IList<AdHocTask> GetTasksToExecute(IList<QueuedTask> pendingQueuedTasks);
    }
}