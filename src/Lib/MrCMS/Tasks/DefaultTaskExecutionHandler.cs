using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MrCMS.Tasks
{
    public class DefaultTaskExecutionHandler : ITaskExecutionHandler
    {
        public DefaultTaskExecutionHandler(ITaskStatusUpdater taskStatusUpdater,
            ILogger<DefaultTaskExecutionHandler> logger)
        {
            _taskStatusUpdater = taskStatusUpdater;
            _logger = logger;
        }

        private readonly ITaskStatusUpdater _taskStatusUpdater;
        private readonly ILogger<DefaultTaskExecutionHandler> _logger;
        public int Priority => -1;

        public IList<AdHocTask> ExtractTasksToHandle(ref IList<AdHocTask> list)
        {
            var newList = list.ToList();
            list.Clear();
            return newList;
        }

        public async Task<IReadOnlyCollection<TaskExecutionResult>> ExecuteTasks(IList<AdHocTask> list)
        {
            if (!list.Any())
                return new List<TaskExecutionResult>();

            _logger.LogInformation($"Executing {list.Count} task(s)");

            await _taskStatusUpdater.BeginExecution(list);
            // var results = list.Select(Execute).ToList();
            // todo - check if this works, otherwise refactor to a loop
            var results = await Task.WhenAll(list.Select(Execute));

            // we are batching these to increase performance (no need for 1 transaction per update)
            await _taskStatusUpdater.CompleteExecution(results);
            return results;
        }

        private async Task<TaskExecutionResult> Execute(AdHocTask executableTask)
        {
            try
            {
                return await executableTask.Execute();
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                return TaskExecutionResult.Failure(executableTask, exception);
            }
        }
    }
}