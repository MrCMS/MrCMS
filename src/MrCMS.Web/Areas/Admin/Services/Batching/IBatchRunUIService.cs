using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public interface IBatchRunUIService
    {
        IList<BatchRunResult> GetResults(BatchRun batchRun);
        Task<int?> Start(int id);
        Task<bool> Pause(int id);
        BatchCompletionStatus GetCompletionStatus(BatchRun batchRun);
    }
}