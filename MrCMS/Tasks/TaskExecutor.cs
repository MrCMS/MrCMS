using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Tasks
{
    public class TaskExecutor : ITaskExecutor
    {
        private readonly ITaskStatusUpdater _taskStatusUpdater;

        public TaskExecutor(ITaskStatusUpdater taskStatusUpdater)
        {
            _taskStatusUpdater = taskStatusUpdater;
        }

        public BatchExecutionResult Execute(IList<IExecutableTask> tasksToExecute)
        {
            var results = tasksToExecute.Select(Execute).ToList();

            return new BatchExecutionResult { Results = results };
        }

        public TaskExecutionResult Execute(IExecutableTask executableTask)
        {
            _taskStatusUpdater.BeginExecution(executableTask);
            var result = executableTask.Execute();
            if (result.Success)
                _taskStatusUpdater.SuccessfulCompletion(executableTask);
            else
                _taskStatusUpdater.FailedExecution(executableTask);
            return result;
        }
    }
}