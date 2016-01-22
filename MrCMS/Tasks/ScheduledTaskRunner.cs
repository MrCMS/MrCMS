using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Tasks.Entities;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Tasks
{
    public class ScheduledTaskRunner : IScheduledTaskRunner
    {
        private readonly SiteSettings _siteSettings;
        private readonly Site _site;
        private readonly IKernel _kernel;
        private readonly ITaskSettingManager _taskSettingManager;

        public ScheduledTaskRunner(SiteSettings siteSettings,
            Site site, IKernel kernel, ITaskSettingManager taskSettingManager)
        {
            _siteSettings = siteSettings;
            _site = site;
            _kernel = kernel;
            _taskSettingManager = taskSettingManager;
        }

        public void TriggerScheduledTasks()
        {
            var pendingScheduledTasks = GetPendingScheduledTasks();

            foreach (var task in pendingScheduledTasks)
            {
                string url = string.Format("{0}/{1}?type={2}&{3}={4}",
                    _site.GetFullDomain.TrimEnd('/'),
                    TaskExecutionController.ExecuteTaskURL,
                    task.TypeName,
                    _siteSettings.TaskExecutorKey,
                    _siteSettings.TaskExecutorPassword);
                new HttpClient().GetAsync(url);
            }
        }

        private List<TaskInfo> GetPendingScheduledTasks()
        {
            DateTime startTime = CurrentRequestData.Now;
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
            var schedulableTask = _kernel.Get(typeObj) as SchedulableTask;
            if (schedulableTask == null)
                return;
            try
            {
                SetStatus(typeObj, TaskExecutionStatus.Executing);
                schedulableTask.Execute();
                SetStatus(typeObj, TaskExecutionStatus.Pending,
                    settings => settings.LastCompleted = CurrentRequestData.Now);
            }
            catch (Exception exception)
            {
                CurrentRequestData.ErrorSignal.Raise(exception);
                SetStatus(typeObj, TaskExecutionStatus.Pending);
            }
        }

        private void SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action = null)
        {
            _taskSettingManager.SetStatus(type, status, action);
        }
    }
}