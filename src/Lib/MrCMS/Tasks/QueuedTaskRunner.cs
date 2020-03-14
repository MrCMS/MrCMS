using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Data;
using MrCMS.Entities.Settings;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services.CloneSite;
using MrCMS.Settings;

namespace MrCMS.Tasks
{
    public class QueuedTaskRunner : IQueuedTaskRunner
    {
        private readonly ITaskBuilder _taskBuilder;
        private readonly ITaskExecutor _taskExecutor;
        private readonly IGlobalRepository<Setting> _repository;
        private readonly ITaskQueuer _taskQueuer;
        private readonly IUrlHelper _urlHelper;
        private readonly ITriggerUrls _triggerUrls;

        public QueuedTaskRunner(ITaskQueuer taskQueuer,
            ITaskBuilder taskBuilder, ITaskExecutor taskExecutor,
            IGlobalRepository<Setting> repository, ITriggerUrls triggerUrls, IUrlHelper urlHelper)
        {
            _taskQueuer = taskQueuer;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
            _repository = repository;
            _triggerUrls = triggerUrls;
            _urlHelper = urlHelper;
        }

        public async Task TriggerPendingTasks()
        {
            var sites = await _taskQueuer.GetPendingQueuedTaskSites();
            var urls = new List<string>();
            foreach (var site in sites)
            {

                var siteSettings = await new SqlConfigurationProvider(_repository, SiteId.GetForSite(site), new NullEventContext()).GetSiteSettings<SiteSettings>();
                urls.Add(_urlHelper.AbsoluteAction("ExecuteQueuedTasks", "TaskExecution",
                    new RouteValueDictionary
                        {[siteSettings.TaskExecutorKey] = siteSettings.TaskExecutorPassword}, site));
            }
            _triggerUrls.Trigger(urls);
        }

        public async Task<BatchExecutionResult> ExecutePendingTasks(CancellationToken token)
        {
            var pendingQueuedTasks = await _taskQueuer.GetPendingQueuedTasks();

            var tasksToExecute = await _taskBuilder.GetTasksToExecute(pendingQueuedTasks);

            return await _taskExecutor.Execute(tasksToExecute, token);
        }

        public async Task<BatchExecutionResult> ExecuteLuceneTasks(CancellationToken token)
        {
            var pendingQueuedTasks = await _taskQueuer.GetPendingLuceneTasks();

            var tasksToExecute = await _taskBuilder.GetTasksToExecute(pendingQueuedTasks);

            return await _taskExecutor.Execute(tasksToExecute, token);
        }
    }
}