using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IControlBatchRun
    {
        Task<bool> Start(BatchRun batchRun);
        Task<bool> Pause(BatchRun batchRun);
    }
}