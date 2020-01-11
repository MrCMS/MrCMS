using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class DefaultTaskExecutionHandler : ITaskExecutionHandler
    {
        public DefaultTaskExecutionHandler(ITaskStatusUpdater taskStatusUpdater, ILogger<DefaultTaskExecutionHandler> logger)
        {
            _taskStatusUpdater = taskStatusUpdater;
            _logger = logger;
        }

        private readonly ITaskStatusUpdater _taskStatusUpdater;
        private readonly ILogger<DefaultTaskExecutionHandler> _logger;
        public int Priority { get { return -1; } }
        public IList<AdHocTask> ExtractTasksToHandle(ref IList<AdHocTask> list)
        {
            var newList = list.ToList();
            list.Clear();
            return newList;
        }

        public async Task<List<TaskExecutionResult>> ExecuteTasks(IList<AdHocTask> list, CancellationToken token)
        {
            await _taskStatusUpdater.BeginExecution(list);
            var results = await Task.WhenAll(list.Select(task => Execute(task, token)));

            // we are batching these to increase performance (no need for 1 transaction per update)
            await _taskStatusUpdater.CompleteExecution(results);
            return results.ToList();
        }

        private async Task<TaskExecutionResult> Execute(AdHocTask executableTask, CancellationToken token)
        {
            try
            {
                return await executableTask.Execute(token);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                return TaskExecutionResult.Failure(executableTask, exception);
            }
        }
    }
}