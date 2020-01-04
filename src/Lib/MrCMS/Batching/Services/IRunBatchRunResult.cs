using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IRunBatchRunResult
    {
        Task<BatchJobExecutionResult> Run(BatchRunResult runResult, CancellationToken token, Stopwatch stopWatch = null);
    }
}