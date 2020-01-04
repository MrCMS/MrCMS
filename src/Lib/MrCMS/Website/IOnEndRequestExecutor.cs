using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public interface IOnEndRequestExecutor
    {
        Task ExecuteTasks(HashSet<EndRequestTask> tasks, CancellationToken token);
    }
}