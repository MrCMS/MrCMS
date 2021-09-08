using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Web.Admin.Services.Batching
{
    public interface IBatchRunUIService
    {
        Task<IList<BatchRunResult>> GetResults(BatchRun batchRun);
        Task<int?> Start(int id);
        Task<bool> Pause(int id);
        Task<BatchCompletionStatus> GetCompletionStatus(BatchRun batchRun);
        Task<BatchRun> Get(int id);
    }
}