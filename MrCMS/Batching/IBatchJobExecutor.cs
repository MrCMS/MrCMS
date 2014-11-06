using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public interface IBatchJobExecutor
    {
        BatchJobExecutionResult Execute(BatchJob batchJob);
    }

    public interface IBatchJobExecutor<in T> where T : BatchJob
    {
        BatchJobExecutionResult Execute(T batchJob);
    }
}