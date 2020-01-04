using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Tasks.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public class ScheduledTaskRunner : IScheduledTaskRunner
    {
        private readonly ILogger<ScheduledTaskRunner> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly ITaskSettingManager _taskSettingManager;

        public ScheduledTaskRunner(
            IServiceProvider serviceProvider, ITaskSettingManager taskSettingManager,
            ILogger<ScheduledTaskRunner> logger)
        {
            _serviceProvider = serviceProvider;
            _taskSettingManager = taskSettingManager;
            _logger = logger;
        }


        public async Task ExecuteTask(string type, CancellationToken token)
        {
            var typeObj = TypeHelper.GetTypeByName(type);
            if (typeObj == null)
            {
                return;
            }

            var schedulableTask = _serviceProvider.GetService(typeObj) as SchedulableTask;
            if (schedulableTask == null)
            {
                return;
            }

            try
            {
                await SetStatus(typeObj, TaskExecutionStatus.Executing);
                await schedulableTask.Execute(token);
                await SetStatus(typeObj, TaskExecutionStatus.Pending,
                     settings => settings.LastCompleted = DateTime.UtcNow);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                await SetStatus(typeObj, TaskExecutionStatus.Pending);
            }
        }

        private async Task SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action = null)
        {
            await _taskSettingManager.SetStatus(type, status, action);
        }
    }
}