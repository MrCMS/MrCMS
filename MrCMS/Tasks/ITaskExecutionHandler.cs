using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskExecutionHandler
    {
        int Priority { get; }
        IList<AdHocTask> ExtractTasksToHandle(ref IList<AdHocTask> list);
        List<TaskExecutionResult> ExecuteTasks(IList<AdHocTask> list);
    }
}