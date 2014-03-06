using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskExecutionHandler
    {
        int Priority { get; }
        IList<IExecutableTask> ExtractTasksToHandle(ref IList<IExecutableTask> list);
        List<TaskExecutionResult> ExecuteTasks(IList<IExecutableTask> list);
    }
}