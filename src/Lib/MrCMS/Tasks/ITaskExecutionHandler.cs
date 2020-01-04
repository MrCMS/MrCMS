using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITaskExecutionHandler
    {
        int Priority { get; }
        IList<AdHocTask> ExtractTasksToHandle(ref IList<AdHocTask> list);
        Task<List<TaskExecutionResult>> ExecuteTasks(IList<AdHocTask> list, CancellationToken token);
    }
}