using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public interface IBatchJobExecutor
    {
        BatchJobExecutionResult Execute(BatchJob batchJob);
    }
}