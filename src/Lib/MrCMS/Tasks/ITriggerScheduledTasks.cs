using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface ITriggerScheduledTasks
    {
        Task Trigger();
    }
}