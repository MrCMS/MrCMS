using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Helpers;
using MrCMS.Settings;
using ISession = NHibernate.ISession;

namespace MrCMS.Tasks
{
    public class QueuedTaskRunner : IQueuedTaskRunner
    {
        private readonly ISession _session;
        private readonly ITaskBuilder _taskBuilder;
        private readonly ITaskExecutor _taskExecutor;
        private readonly ITaskQueuer _taskQueuer;
        private readonly IUrlHelper _urlHelper;
        private readonly ITriggerUrls _triggerUrls;

        public QueuedTaskRunner(ITaskQueuer taskQueuer,
            ITaskBuilder taskBuilder, ITaskExecutor taskExecutor,
            ISession session, ITriggerUrls triggerUrls, IUrlHelper urlHelper)
        {
            _taskQueuer = taskQueuer;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
            _session = session;
            _triggerUrls = triggerUrls;
            _urlHelper = urlHelper;
        }

        public void TriggerPendingTasks()
        {
            var sites = _taskQueuer.GetPendingQueuedTaskSites();
            _triggerUrls.Trigger(sites.Select(site =>
            {
                var siteSettings = new SqlConfigurationProvider(_session, site).GetSiteSettings<SiteSettings>();

                return _urlHelper.AbsoluteAction("ExecuteQueuedTasks", "TaskExecution",
                    new RouteValueDictionary {[siteSettings.TaskExecutorKey] = siteSettings.TaskExecutorPassword}, site);
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