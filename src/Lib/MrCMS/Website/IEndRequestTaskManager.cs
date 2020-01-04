using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public interface IEndRequestTaskManager
    {
        void AddTask(EndRequestTask task);
        Task ExecuteTasks(CancellationToken token);
        HashSet<EndRequestTask> GetTasks();
    }
}