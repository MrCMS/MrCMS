using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskExecutor
    {
        BatchExecutionResult Execute(IList<IExecutableTask> tasksToExecute);
    }
}