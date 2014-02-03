using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Tasks
{
    public class DefaultTaskExecutionHandler : ITaskExecutionHandler
    {
        public DefaultTaskExecutionHandler(ITaskStatusUpdater taskStatusUpdater)
        {
            _taskStatusUpdater = taskStatusUpdater;
        }

        private readonly ITaskStatusUpdater _taskStatusUpdater;
        public int Priority { get { return -1; } }
        public IList<IExecutableTask> ExtractTasksToHandle(ref IList<IExecutableTask> list)
        {
            var newList = list.ToList();
            list.Clear();
            return newList;
        }

        public List<TaskExecutionResult> ExecuteTasks(IList<IExecutableTask> list)
        {
            return list.Select(Execute).ToList();
        }

        private TaskExecutionResult Execute(IExecutableTask executableTask)
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