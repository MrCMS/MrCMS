using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Data;

namespace MrCMS.Tasks
{
    public class TaskBuilder : ITaskBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IRepository<QueuedTask> _repository;

        private readonly ILogger<TaskBuilder> _logger;

        public TaskBuilder(IServiceProvider serviceProvider, IRepository<QueuedTask> repository, ILogger<TaskBuilder> logger)
        {
            _serviceProvider = serviceProvider;
            _repository = repository;
            _logger = logger;
        }

        public async Task<IList<AdHocTask>> GetTasksToExecute(IList<QueuedTask> pendingQueuedTasks)
        {
            var executableTasks = new List<AdHocTask>();
            var failedTasks = new List<QueuedTask>();
            foreach (var queuedTask in pendingQueuedTasks)
            {
                try
                {
                    var task = GetTask(queuedTask);
                    executableTasks.Add(task);
                }
                catch (Exception exception)
                {
                    _logger.Log(LogLevel.Error, exception, exception.Message);
                    failedTasks.Add(queuedTask);
                }
            }
            if (failedTasks.Any())
            {
                failedTasks.ForEach(task =>
                {
                    task.Status = TaskExecutionStatus.Failed;
                    task.FailedAt = DateTime.UtcNow;
                });
                await _repository.UpdateRange(failedTasks);
            }
            return executableTasks;
        }

        private AdHocTask GetTask(QueuedTask queuedTask)
        {
            var task = _serviceProvider.GetService(queuedTask.GetTaskType()) as AdHocTask;
            task.Site = queuedTask.Site;
            task.Entity = queuedTask;
            task.SetData(queuedTask.Data);
            return task;
        }

    }
}