using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface ITaskStatusUpdater
    {
        void BeginExecution(IExecutableTask executableTask);
        void BeginExecution(IEnumerable<IExecutableTask> executableTasks);
        void SuccessfulCompletion(IExecutableTask executableTask);
        void SuccessfulCompletion(IEnumerable<IExecutableTask> executableTasks);
        void FailedExecution(IExecutableTask executableTask);
        void FailedExecution(IEnumerable<IExecutableTask> executableTasks);
    }
}