using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Tasks
{
    public interface ITriggerQueuedTasksOnSites
    {
        Task Trigger(params Site[] sites);
    }
}