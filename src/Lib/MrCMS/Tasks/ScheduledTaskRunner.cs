using Microsoft.Extensions.Logging;
using MrCMS.Tasks.Entities;
using System;
using System.Threading.Tasks;
using Quartz;

namespace MrCMS.Tasks
{
    [DisallowConcurrentExecution]
    public class ScheduledTaskRunner<T> : IJob where T : SchedulableTask
    {
        private readonly ILogger<ScheduledTaskRunner<T>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITaskSettingManager _taskSettingManager;

        public ScheduledTaskRunner(
            IServiceProvider serviceProvider, ITaskSettingManager taskSettingManager,
            ILogger<ScheduledTaskRunner<T>> logger)
        {
            _serviceProvider = serviceProvider;
            _taskSettingManager = taskSettingManager;
            _logger = logger;
        }

        private async Task SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action = null)
        {
            await _taskSettingManager.SetStatus(type, status, action);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"{DateTime.UtcNow} - Executing task: {typeof(T).Name}");
            var serviceType = typeof(T);

            try
            {
                if (!(_serviceProvider.GetService(serviceType) is SchedulableTask schedulableTask))
                {
                    return;
                }

                await SetStatus(serviceType, TaskExecutionStatus.Executing,
                    settings => settings.LastStarted = DateTime.UtcNow);
                await schedulableTask.Execute();
                await SetStatus(serviceType, TaskExecutionStatus.Pending,
                    settings => settings.LastCompleted = DateTime.UtcNow);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                await SetStatus(serviceType, TaskExecutionStatus.Pending);
            }
            finally
            {
                _logger.LogInformation(
                    $"{DateTime.UtcNow} - Executed task: {typeof(T).Name} - Duration: {context.JobRunTime}");
            }
        }
    }
}