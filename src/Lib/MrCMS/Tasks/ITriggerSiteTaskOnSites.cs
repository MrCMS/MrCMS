using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public interface ITriggerSiteTaskOnSites
    {
        Task Trigger<T>(params Site[] sites) where T : SiteTask;
    }
}