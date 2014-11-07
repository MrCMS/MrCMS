using System.Collections.Generic;
using MrCMS.Batching.Entities;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public interface IBatchRunUIService
    {
        IList<BatchRunResult> GetResults(BatchRun batchRun);
        int? Start(BatchRun run);
        int? ExecuteNextTask(BatchRun run);
        bool Pause(BatchRun run);
    }
}