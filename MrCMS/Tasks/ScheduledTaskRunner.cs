using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks.Entities;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Tasks
{
    public class ScheduledTaskRunner : IScheduledTaskRunner
    {
        private readonly SiteSettings _siteSettings;
        private readonly Site _site;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITaskSettingManager _taskSettingManager;
        private readonly ITriggerUrls _triggerUrls;
        private readonly ILogger<ScheduledTaskRunner> _logger;
        private readonly IGetNowForSite _getNowForSite;

        public ScheduledTaskRunner(SiteSettings siteSettings,
            Site site, IServiceProvider serviceProvider, ITaskSettingManager taskSettingManager, ITriggerUrls triggerUrls,
            ILogger<ScheduledTaskRunner> logger,IGetNowForSite getNowForSite)
        {
            _siteSettings = siteSettings;
            _site = site;
            _serviceProvider = serviceProvider;
            _taskSettingManager = taskSettingManager;
            _triggerUrls = triggerUrls;
            _logger = logger;
            _getNowForSite = getNowForSite;
        }

        public void TriggerScheduledTasks()
        {
            _triggerUrls.Trigger(GetPendingScheduledTasks()
                .Select(task => string.Format("{0}/{1}?type={2}&{3}={4}",
                    _site.GetFullDomain.TrimEnd('/'),
                    TaskExecutionController.ExecuteTaskURL,
                    task.TypeName,
                    _siteSettings.TaskExecutorKey,
                    _siteSettings.TaskExecutorPassword)));
        }

        private List<TaskInfo> GetPendingScheduledTasks()
        {
            DateTime startTime = _getNowForSite.Now;
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

        private void SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action = null)
        {
            _taskSettingManager.SetStatus(type, status, action);
        }
    }
}