using System.Diagnostics;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public interface IRunBatchRunResult
    {
        Task<BatchJobExecutionResult> Run(BatchRunResult runResult, Stopwatch stopWatch = null);
    }
}