using System.Linq;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ICreateBatchRun
    {
        BatchRun Create(Batch batch);
    }
}