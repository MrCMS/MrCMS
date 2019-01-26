using MrCMS.Batching.Entities;
using MrCMS.Entities;

namespace MrCMS.Batching
{
    public interface ISetBatchJobExecutionStatus
    {
        void Starting<T>(T entity) where T : SystemEntity, IHaveJobExecutionStatus;
        void Complete<T>(T entity, BatchJobExecutionResult result) where T : SystemEntity, IHaveJobExecutionStatus;
    }
}