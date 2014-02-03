using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Tasks
{
    public class TaskExecutor : ITaskExecutor
    {
        private readonly IEnumerable<ITaskExecutionHandler> _executionHandlers;

        public TaskExecutor(IEnumerable<ITaskExecutionHandler> executionHandlers)
        {
            _executionHandlers = executionHandlers;
        }

        public BatchExecutionResult Execute(IList<IExecutableTask> tasksToExecute)
        {
            var results = new List<TaskExecutionResult>();
            foreach (var handler in _executionHandlers.OrderByDescending(handler => handler.Priority))
            {
                var tasks = handler.ExtractTasksToHandle(ref tasksToExecute);
                results.AddRange(handler.ExecuteTasks(tasks));
            }
            return new BatchExecutionResult { Results = results };
        }
    }
}