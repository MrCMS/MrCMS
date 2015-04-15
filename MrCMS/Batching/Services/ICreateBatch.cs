using System.Collections.Generic;
using MrCMS.Batching.Entities;
using MrCMS.Services.FileMigration;
using Newtonsoft.Json;

namespace MrCMS.Batching.Services
{
    public interface ICreateBatch
    {
        BatchCreationResult Create(IEnumerable<BatchJob> jobs);
    }
}