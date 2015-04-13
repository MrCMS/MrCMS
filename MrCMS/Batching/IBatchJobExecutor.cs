using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public interface IBatchJobExecutor
    {
        Task<BatchJobExecutionResult> Execute(BatchJob batchJob);
        bool UseAsync { get; }
    }
}