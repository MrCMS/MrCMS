using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public interface IGetInternalTaskUrl
    {
        string GetSiteTaskUrl<T>(Site site) where T : SiteTask;
        string GetQueuedTaskUrl(Site site);
    }
}