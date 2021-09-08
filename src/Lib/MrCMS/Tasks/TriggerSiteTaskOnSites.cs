using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public class TriggerSiteTaskOnSites : ITriggerSiteTaskOnSites
    {
        private readonly ITriggerUrls _triggerUrls;
        private readonly IGetInternalTaskUrl _getInternalTaskUrl;

        public TriggerSiteTaskOnSites(ITriggerUrls triggerUrls, IGetInternalTaskUrl getInternalTaskUrl)
        {
            _triggerUrls = triggerUrls;
            _getInternalTaskUrl = getInternalTaskUrl;
        }

        public async Task Trigger<T>(params Site[] sites) where T : SiteTask
        {
            await _triggerUrls.Trigger(sites.Select(site => _getInternalTaskUrl.GetSiteTaskUrl<T>(site)));
        }
    }
}