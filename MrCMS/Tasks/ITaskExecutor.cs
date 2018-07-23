using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskExecutor
    {
        BatchExecutionResult Execute(IList<AdHocTask> tasksToExecute);
        BatchExecutionResult Execute(AdHocTask task);
    }
}