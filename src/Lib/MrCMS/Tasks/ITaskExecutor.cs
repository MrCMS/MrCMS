using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITaskExecutor
    {
        Task<BatchExecutionResult> Execute(IList<AdHocTask> tasksToExecute);
        Task<BatchExecutionResult> Execute(AdHocTask task);
    }
}