using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IExecuteNextBatchJob
    {
        bool Execute(BatchRun batchRun);
    }
}