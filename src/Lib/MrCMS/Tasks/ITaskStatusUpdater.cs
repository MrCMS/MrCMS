using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITaskStatusUpdater
    {
        Task BeginExecution(IEnumerable<AdHocTask> executableTasks);
        Task CompleteExecution(IEnumerable<TaskExecutionResult> results);
    }
}