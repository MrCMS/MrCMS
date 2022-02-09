using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ICreateBatchRun
    {
        Task<BatchRun> Create(Batch batch);
    }
}