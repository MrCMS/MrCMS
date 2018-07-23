using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskStatusUpdater
    {
        void BeginExecution(IEnumerable<AdHocTask> executableTasks);
        void CompleteExecution(IEnumerable<TaskExecutionResult> results);
    }
}