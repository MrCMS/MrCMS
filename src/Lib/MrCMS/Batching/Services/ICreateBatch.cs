using System.Collections.Generic;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface ICreateBatch
    {
        BatchCreationResult Create(IEnumerable<BatchJob> jobs);
    }
}