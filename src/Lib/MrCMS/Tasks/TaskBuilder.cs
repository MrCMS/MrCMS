using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskBuilder : ITaskBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IStatelessSession _session;
        private readonly ILogger<TaskBuilder> _logger;

        public TaskBuilder(IServiceProvider serviceProvider, IStatelessSession session, ILogger<TaskBuilder> logger)
        {
            _serviceProvider = serviceProvider;
            _session = session;
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
                await _session.TransactAsync(async session =>
                {
                    foreach (var task in failedTasks)
                    {
                        task.Status = TaskExecutionStatus.Failed;
                        task.FailedAt = DateTime.UtcNow;
                        await session.UpdateAsync(task);
                    }
                });
            }

            return executableTasks;
        }

        private AdHocTask GetTask(QueuedTask queuedTask)
        {
            var serviceType = queuedTask.GetTaskType();
            if (serviceType == null)
                throw new Exception($"Unknown type: {queuedTask.Type}");
            
            var task = _serviceProvider.GetService(serviceType) as AdHocTask;
            task.Site = queuedTask.Site;
            task.Entity = queuedTask;
            task.SetData(queuedTask.Data);
            return task;
        }
    }
}