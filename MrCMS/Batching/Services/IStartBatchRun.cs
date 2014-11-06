using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IStartBatchRun
    {
        void Start(BatchRun batchRun);
    }
}