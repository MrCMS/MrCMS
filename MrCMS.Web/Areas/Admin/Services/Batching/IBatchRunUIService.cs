using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public interface IBatchRunUIService
    {
        IList<BatchRunResult> GetResults(BatchRun batchRun);
        int? Start(BatchRun run);
        Task<int?> ExecuteNextTask(BatchRun run);
        bool Pause(BatchRun run);
        BatchCompletionStatus GetCompletionStatus(BatchRun batchRun);
        void ExecuteRequestForNextTask(BatchRun run);
    }
}