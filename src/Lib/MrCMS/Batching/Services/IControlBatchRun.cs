using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IControlBatchRun
    {
        bool Start(BatchRun batchRun);
        bool Pause(BatchRun batchRun);
    }
}