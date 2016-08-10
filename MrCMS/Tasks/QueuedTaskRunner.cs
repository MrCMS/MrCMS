using System.Linq;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Tasks
{
    public class QueuedTaskRunner : IQueuedTaskRunner
    {
        private readonly IStatelessSession _session;
        private readonly ITaskBuilder _taskBuilder;
        private readonly ITaskExecutor _taskExecutor;
        private readonly ITaskQueuer _taskQueuer;
        private readonly ITriggerUrls _triggerUrls;

        public QueuedTaskRunner(ITaskQueuer taskQueuer, ITaskBuilder taskBuilder, ITaskExecutor taskExecutor,
            IStatelessSession session, ITriggerUrls triggerUrls)
        {
            _taskQueuer = taskQueuer;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
            _session = session;
            _triggerUrls = triggerUrls;
        }

        public void TriggerPendingTasks()
        {
            var sites = _taskQueuer.GetPendingQueuedTaskSites();
            _triggerUrls.Trigger(sites.Select(site =>
            {
                var siteSettings = new SqlConfigurationProvider(_session, site).GetSiteSettings<SiteSettings>();

                return string.Format("{0}/{1}?{2}={3}",
                    site.GetFullDomain.TrimEnd('/'),
                    TaskExecutionController.ExecuteQueuedTasksURL,
                    siteSettings.TaskExecutorKey,
                    siteSettings.TaskExecutorPassword);
            }));
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