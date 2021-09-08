using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public interface IEndRequestTaskManager
    {
        void AddTask(EndRequestTask task);
        Task ExecuteTasks();
        HashSet<EndRequestTask> GetTasks();
    }
}