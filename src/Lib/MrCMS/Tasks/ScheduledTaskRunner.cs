using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Tasks.Entities;
using System;

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


        public void ExecuteTask(string type)
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
                SetStatus(typeObj, TaskExecutionStatus.Executing);
                schedulableTask.Execute();
                SetStatus(typeObj, TaskExecutionStatus.Pending,
                    settings => settings.LastCompleted = DateTime.UtcNow);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                SetStatus(typeObj, TaskExecutionStatus.Pending);
            }
        }

        private void SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action = null)
        {
            _taskSettingManager.SetStatus(type, status, action);
        }
    }
}