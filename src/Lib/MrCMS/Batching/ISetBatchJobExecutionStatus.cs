using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Entities;

namespace MrCMS.Batching
{
    public interface ISetBatchJobExecutionStatus
    {
        Task Starting<T>(T entity) where T : SystemEntity, IHaveJobExecutionStatus;
        Task Complete<T>(T entity, BatchJobExecutionResult result) where T : SystemEntity, IHaveJobExecutionStatus;
    }
}