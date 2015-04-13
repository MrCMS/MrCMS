using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IExecuteNextBatchJob
    {
        Task<bool> Execute(BatchRun batchRun);
    }
}