using System.Diagnostics;
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

        public bool Execute(BatchRun batchRun)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = _getNextJobToRun.Get(batchRun);
            var runResult = result.Result;
            if (runResult == null)
            {
                if (result.Complete)
                    _setRunStatus.Complete(batchRun);
                if (result.Paused)
                    _setRunStatus.Paused(batchRun);
                return false;
            }

            if (runResult.BatchJob == null)
                _setBatchJobExecutionStatus.Complete(runResult,
                    BatchJobExecutionResult.Failure("No job associated to result"));

            _runBatchRunResult.Run(runResult, stopWatch);

            return true;
        }

    }
}