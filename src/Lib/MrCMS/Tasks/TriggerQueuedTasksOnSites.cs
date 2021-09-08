using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public class TriggerQueuedTasksOnSites : ITriggerQueuedTasksOnSites
    {
        private readonly ITriggerUrls _triggerUrls;
        private readonly IGetInternalTaskUrl _getInternalTaskUrl;

        public TriggerQueuedTasksOnSites(ITriggerUrls triggerUrls, IGetInternalTaskUrl getInternalTaskUrl)
        {
            _triggerUrls = triggerUrls;
            _getInternalTaskUrl = getInternalTaskUrl;
        }

        public async Task Trigger(params Site[] sites)
        {
            await _triggerUrls.Trigger(sites.Select(site => _getInternalTaskUrl.GetQueuedTaskUrl(site)));
        }
    }
}