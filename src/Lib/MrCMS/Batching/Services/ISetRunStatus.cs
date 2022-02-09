using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ISetRunStatus
    {
        Task Complete(BatchRun batchRun);
        Task Paused(BatchRun batchRun);
    }
}