using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Batching.Services
{
    public class ExecuteNextBatchJob : IExecuteNextBatchJob
    {
        private readonly IGetNextJobToRun _getNextJobToRun;
        private readonly ISetBatchJobExecutionStatus _setBatchJobExecutionStatus;
        private readonly ISetRunStatus _setRunStatus;
        private readonly IRunBatchRunResult _runBatchRunResult;

        public ExecuteNextBatchJob(IGetNextJobToRun getNextJobToRun,
            ISetBatchJobExecutionStatus setBatchJobExecutionStatus, ISetRunStatus setRunStatus, IRunBatchRunResult runBatchRunResult)
        {
            _getNextJobToRun = getNextJobToRun;
            _setBatchJobExecutionStatus = setBatchJobExecutionStatus;
            _setRunStatus = setRunStatus;
            _runBatchRunResult = runBatchRunResult;
        }

        public async Task<bool> Execute(BatchRun batchRun, CancellationToken token)
        {
            if (batchRun == null)
                return false;
            var stopWatch = Stopwatch.StartNew();
            var result = _getNextJobToRun.Get(batchRun);
            var runResult = result.Result;
            if (runResult == null)
            {
                if (result.Complete)
                    await _setRunStatus.Complete(batchRun);
                if (result.Paused)
                    await _setRunStatus.Paused(batchRun);
                return false;
            }

            if (runResult.BatchJob == null)
                await _setBatchJobExecutionStatus.Complete(runResult,
                                   BatchJobExecutionResult.Failure("No job associated to result"));

            await _runBatchRunResult.Run(runResult, token, stopWatch);

            return true;
        }

    }
}