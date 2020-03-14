using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Tasks
{
    public class TriggerScheduledTasks : ITriggerScheduledTasks
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ITriggerUrls _triggerUrls;
        private readonly IUrlHelper _urlHelper;
        private readonly IGetPendingScheduledTasks _getPendingScheduledTasks;

        public TriggerScheduledTasks(IConfigurationProvider configurationProvider, ITriggerUrls triggerUrls, IUrlHelper urlHelper, IGetPendingScheduledTasks getPendingScheduledTasks)
        {
            _configurationProvider = configurationProvider;
            _triggerUrls = triggerUrls;
            _urlHelper = urlHelper;
            _getPendingScheduledTasks = getPendingScheduledTasks;
        }

        public async Task Trigger()
        {
            var tasks = await _getPendingScheduledTasks.GetTasks();
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            _triggerUrls.Trigger(tasks
                .Select(task => _urlHelper.AbsoluteAction("ExecuteTask", "TaskExecution",
                    new RouteValueDictionary
                    {
                        ["type"] = task.TypeName,
                        [siteSettings.TaskExecutorKey] = siteSettings.TaskExecutorPassword
                    })));
        }
    }
}