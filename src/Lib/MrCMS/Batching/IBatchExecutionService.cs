using System;
using System.Threading.Tasks;

namespace MrCMS.Batching
{
    public interface IBatchExecutionService
    {
        Task<int?> ExecuteNextTask(Guid id);
        Task ExecuteRequestForNextTask(Guid id);
    }
}