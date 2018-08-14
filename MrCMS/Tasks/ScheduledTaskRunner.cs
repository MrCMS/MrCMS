using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks.Entities;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class ScheduledTaskRunner : IScheduledTaskRunner
    {
        private readonly IGetNowForSite _getNowForSite;
        private readonly ILogger<ScheduledTaskRunner> _logger;

        private readonly IServiceProvider _serviceProvider;

        // TODO: refactor this, as there are too many dependencies - split into parts
        private readonly SiteSettings _siteSettings;
        private readonly ITaskSettingManager _taskSettingManager;
        private readonly ITriggerUrls _triggerUrls;
        private readonly IUrlHelper _urlHelper;

        public ScheduledTaskRunner(SiteSettings siteSettings,
            IServiceProvider serviceProvider, ITaskSettingManager taskSettingManager,
            ITriggerUrls triggerUrls,
            ILogger<ScheduledTaskRunner> logger, IGetNowForSite getNowForSite, IUrlHelper urlHelper)
        {
            _siteSettings = siteSettings;
            _serviceProvider = serviceProvider;
            _taskSettingManager = taskSettingManager;
            _triggerUrls = triggerUrls;
            _logger = logger;
            _getNowForSite = getNowForSite;
            _urlHelper = urlHelper;
        }

        public void TriggerScheduledTasks()
        {
            _triggerUrls.Trigger(GetPendingScheduledTasks()
                .Select(task => _urlHelper.AbsoluteAction("ExecuteTask", "TaskExecution",
                    new RouteValueDictionary
                    {
                        ["type"] = task.TypeName,
                        [_siteSettings.TaskExecutorKey] = _siteSettings.TaskExecutorPassword
                    })));
        }


        public void ExecuteTask(string type)
        {
            var typeObj = TypeHelper.GetTypeByName(type);
            var schedulableTask = _serviceProvider.GetService(typeObj) as SchedulableTask;
            if (schedulableTask == null)
                return;
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

        private List<TaskInfo> GetPendingScheduledTasks()
        {
            var startTime = _getNowForSite.Now;
            var scheduledTasks =
                _taskSettingManager.GetInfo()
                    .Where(task =>
                        task.Enabled && task.Status == TaskExecutionStatus.Pending &&
                        (task.LastCompleted < startTime.AddSeconds(-task.FrequencyInSeconds) ||
                         task.LastCompleted == null))
                    .ToList();
            _taskSettingManager.StartTasks(scheduledTasks, startTime);
            return scheduledTasks;
        }

        private void SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action = null)
        {
            _taskSettingManager.SetStatus(type, status, action);
        }
    }
}