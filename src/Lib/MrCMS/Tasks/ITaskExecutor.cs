using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITaskExecutor
    {
        Task<BatchExecutionResult> Execute(IList<AdHocTask> tasksToExecute, CancellationToken token);
        Task<BatchExecutionResult> Execute(AdHocTask task, CancellationToken token);
    }
}