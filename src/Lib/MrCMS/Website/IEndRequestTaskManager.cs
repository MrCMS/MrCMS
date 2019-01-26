using System.Collections.Generic;

namespace MrCMS.Website
{
    public interface IEndRequestTaskManager
    {
        void AddTask(EndRequestTask task);
        void ExecuteTasks();
        HashSet<EndRequestTask> GetTasks();
    }
}