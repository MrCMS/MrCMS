using System;
using System.Net.Http;
using MrCMS.Settings;

namespace MrCMS.Tasks
{
    public class QueuedTaskRunner : IQueuedTaskRunner
    {
        private readonly ITaskQueuer _taskQueuer;
        private readonly ITaskBuilder _taskBuilder;
        private readonly ITaskExecutor _taskExecutor;

        public QueuedTaskRunner(ITaskQueuer taskQueuer, ITaskBuilder taskBuilder, ITaskExecutor taskExecutor)
        {
            _taskQueuer = taskQueuer;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
        }

        public void TriggerPendingTasks()
        {
            var sites = _taskQueuer.GetPendingQueuedTaskSites();

            foreach (var site in sites)
            {
                var siteSettings = new ConfigurationProvider(site, null).GetSiteSettings<SiteSettings>();

                string url = string.Format("{0}/{1}?{2}={3}",
                    site.GetFullDomain.TrimEnd('/'),
                    TaskExecutionController.ExecuteQueuedTasksURL,
                    siteSettings.TaskExecutorKey,
                    siteSettings.TaskExecutorPassword);
                new HttpClient().GetAsync(url);
            }
        }

        public BatchExecutionResult ExecutePendingTasks()
        {
            var pendingQueuedTasks = _taskQueuer.GetPendingQueuedTasks();

            var tasksToExecute = _taskBuilder.GetTasksToExecute(pendingQueuedTasks);

            return _taskExecutor.Execute(tasksToExecute);
        }

        public BatchExecutionResult ExecuteLuceneTasks()
        {
            var pendingQueuedTasks = _taskQueuer.GetPendingLuceneTasks();

            var tasksToExecute = _taskBuilder.GetTasksToExecute(pendingQueuedTasks);

            return _taskExecutor.Execute(tasksToExecute);
        }
    }
}