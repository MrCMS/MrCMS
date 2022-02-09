using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ICreateBatch
    {
        Task<BatchCreationResult> Create(IEnumerable<BatchJob> jobs);
    }
}