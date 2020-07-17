using System.Collections.Generic;
using MrCMS.Batching.Entities;

namespace MrCMS.Web.Admin.Services.Batching
{
    public interface IBatchRunUIService
    {
        IList<BatchRunResult> GetResults(BatchRun batchRun);
        int? Start(int id);
        bool Pause(int id);
        BatchCompletionStatus GetCompletionStatus(BatchRun batchRun);
    }
}