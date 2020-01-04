using System;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public interface IScheduledTaskRunner
    {
        Task ExecuteTask(string type, CancellationToken token);
    }
}