using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IBatchJobExecutionService
    {
        BatchJobExecutionResult Execute(BatchJob batchJob);
    }
}