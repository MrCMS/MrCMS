using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class TaskExecutor : ITaskExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskExecutor(IServiceProvider  serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public BatchExecutionResult Execute(IList<AdHocTask> tasksToExecute)
        {
            var results = new List<TaskExecutionResult>();
            foreach (var handler in GetHandlers())
            {
                var tasks = handler.ExtractTasksToHandle(ref tasksToExecute);
                results.AddRange(handler.ExecuteTasks(tasks));
            }
            return new BatchExecutionResult { Results = results };
        }

        private IEnumerable<ITaskExecutionHandler> GetHandlers()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<ITaskExecutionHandler>();
            return types.Select(_serviceProvider.GetService)
                .OfType<ITaskExecutionHandler>()
                .OrderByDescending(handler => handler.Priority);
        }

        public BatchExecutionResult Execute(AdHocTask task)
        {
            if (task != null)
            {
                IList<AdHocTask> tasksToExecute = new List<AdHocTask> {task};
                foreach (var handler in GetHandlers())
                {
                    var tasks = handler.ExtractTasksToHandle(ref tasksToExecute);
                    if (tasks.Any())
                        return new BatchExecutionResult {Results = handler.ExecuteTasks(tasks)};
                }
            }
            return new BatchExecutionResult();
        }
    }
}