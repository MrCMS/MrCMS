using System.Diagnostics;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IRunBatchRunResult
    {
        BatchJobExecutionResult Run(BatchRunResult runResult, Stopwatch stopWatch = null);
    }
}