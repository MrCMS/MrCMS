using System.Linq;
using System.Threading.Tasks;
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
        private readonly IConfigurationProviderFactory _factory;
        private readonly ITriggerUrls _triggerUrls;

        public QueuedTaskRunner(ITaskQueuer taskQueuer,
            ITaskBuilder taskBuilder, ITaskExecutor taskExecutor,
            ISession session, ITriggerUrls triggerUrls, IUrlHelper urlHelper, IConfigurationProviderFactory factory)
        {
            _taskQueuer = taskQueuer;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
            _session = session;
            _triggerUrls = triggerUrls;
            _urlHelper = urlHelper;
            _factory = factory;
        }

        public async Task TriggerPendingTasks()
        {
            var sites = await _taskQueuer.GetPendingQueuedTaskSites();
            await _triggerUrls.Trigger(sites.Select(site =>
            {
                var siteSettings = _factory.GetForSite(site).GetSiteSettings<SiteSettings>();

                return _urlHelper.AbsoluteAction("ExecuteQueuedTasks", "TaskExecution",
                    new RouteValueDictionary
                        {[siteSettings.TaskExecutorKey] = siteSettings.TaskExecutorPassword}, site);
            }));
        }

        public async Task<BatchExecutionResult> ExecutePendingTasks()
        {
            var pendingQueuedTasks = await _taskQueuer.GetPendingQueuedTasks();

            var tasksToExecute = await _taskBuilder.GetTasksToExecute(pendingQueuedTasks);

            return await _taskExecutor.Execute(tasksToExecute);
        }
    }
}