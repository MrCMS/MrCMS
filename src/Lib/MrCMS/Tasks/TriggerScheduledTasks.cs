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
        private readonly SiteSettings _siteSettings;
        private readonly ITriggerUrls _triggerUrls;
        private readonly IUrlHelper _urlHelper;
        private readonly IGetPendingScheduledTasks _getPendingScheduledTasks;

        public TriggerScheduledTasks(SiteSettings siteSettings, ITriggerUrls triggerUrls, IUrlHelper urlHelper, IGetPendingScheduledTasks getPendingScheduledTasks)
        {
            _siteSettings = siteSettings;
            _triggerUrls = triggerUrls;
            _urlHelper = urlHelper;
            _getPendingScheduledTasks = getPendingScheduledTasks;
        }

        public async Task Trigger()
        {
            var tasks = await _getPendingScheduledTasks.GetTasks();
            _triggerUrls.Trigger(tasks
                .Select(task => _urlHelper.AbsoluteAction("ExecuteTask", "TaskExecution",
                    new RouteValueDictionary
                    {
                        ["type"] = task.TypeName,
                        [_siteSettings.TaskExecutorKey] = _siteSettings.TaskExecutorPassword
                    })));
        }
    }
}