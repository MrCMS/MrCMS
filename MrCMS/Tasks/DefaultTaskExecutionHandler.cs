using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Website;

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
        public IList<AdHocTask> ExtractTasksToHandle(ref IList<AdHocTask> list)
        {
            var newList = list.ToList();
            list.Clear();
            return newList;
        }

        public List<TaskExecutionResult> ExecuteTasks(IList<AdHocTask> list)
        {
            _taskStatusUpdater.BeginExecution(list);
            var results = list.Select(Execute).ToList();

            // we are batching these to increase performance (no need for 1 transaction per update)
            _taskStatusUpdater.CompleteExecution(results);
            return results;
        }

        private TaskExecutionResult Execute(AdHocTask executableTask)
        {
            try
            {
                return executableTask.Execute();
            }
            catch (Exception exception)
            {
                CurrentRequestData.ErrorSignal.Raise(exception);
                return TaskExecutionResult.Failure(executableTask, exception);
            }
        }
    }
}