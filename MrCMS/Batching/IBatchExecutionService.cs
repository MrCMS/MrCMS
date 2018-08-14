using System;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public interface IBatchExecutionService
    {
        Task<int?> ExecuteNextTask(Guid id);
        void ExecuteRequestForNextTask(Guid id);
    }
}