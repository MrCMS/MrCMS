using System;
using System.Net.Http;
using MrCMS.Settings;

namespace MrCMS.Tasks
{
    public class ScheduledTaskRunner : IScheduledTaskRunner
    {
        private readonly ITaskQueuer _taskQueuer;
        private readonly SiteSettings _siteSettings;
        private readonly ITaskBuilder _taskBuilder;
        private readonly ITaskExecutor _taskExecutor;

        public ScheduledTaskRunner(ITaskQueuer taskQueuer, SiteSettings siteSettings, ITaskBuilder taskBuilder,ITaskExecutor taskExecutor )
        {
            _taskQueuer = taskQueuer;
            _siteSettings = siteSettings;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
        }

        public void TriggerScheduledTasks()
        {
            var pendingScheduledTasks = _taskQueuer.GetPendingScheduledTasks();

            foreach (var task in pendingScheduledTasks)
            {
                var site = task.Site;
                string url = string.Format("{0}/{1}?id={2}&{3}={4}",
                    site.GetFullDomain.TrimEnd('/'),
                    TaskExecutionController.ExecuteTaskURL,
                    task.Guid,
                    _siteSettings.TaskExecutorKey,
                    _siteSettings.TaskExecutorPassword);
                new HttpClient().GetAsync(url);
            }
        }

        public void ExecuteTask(Guid id)
        {
            var task = _taskBuilder.GetTask(id);
            _taskExecutor.Execute(task);
        }
    }
}