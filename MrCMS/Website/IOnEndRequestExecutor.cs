using System.Collections.Generic;

namespace MrCMS.Website
{
    public interface IOnEndRequestExecutor
    {
        void ExecuteTasks(HashSet<EndRequestTask> tasks);
    }
}