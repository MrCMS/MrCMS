using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ISetRunStatus
    {
        void Complete(BatchRun batchRun);
        void Paused(BatchRun batchRun);
    }
}