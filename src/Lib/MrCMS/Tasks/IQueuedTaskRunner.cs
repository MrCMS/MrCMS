using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface IQueuedTaskRunner
    {
        Task TriggerPendingTasks();

        Task<BatchExecutionResult> ExecutePendingTasks(CancellationToken token);
        Task<BatchExecutionResult> ExecuteLuceneTasks(CancellationToken token);
    }
}