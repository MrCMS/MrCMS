using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public class RunBatchRunResult : IRunBatchRunResult
    {
        private readonly IBatchJobExecutionService _batchJobExecutionService;
        private readonly ISetBatchJobExecutionStatus _setBatchJobExecutionStatus;

        public RunBatchRunResult(IBatchJobExecutionService batchJobExecutionService, ISetBatchJobExecutionStatus setBatchJobExecutionStatus)
        {
            _batchJobExecutionService = batchJobExecutionService;
            _setBatchJobExecutionStatus = setBatchJobExecutionStatus;
        }

        public async Task<BatchJobExecutionResult> Run(BatchRunResult runResult, CancellationToken token,
            Stopwatch stopWatch = null)
        {
            stopWatch = stopWatch ?? Stopwatch.StartNew();

            await _setBatchJobExecutionStatus.Starting(runResult);

            var batchJobExecutionResult = await _batchJobExecutionService.Execute(runResult.BatchJob, token);

            runResult.MillisecondsTaken = Convert.ToDecimal(stopWatch.Elapsed.TotalMilliseconds);
            await _setBatchJobExecutionStatus.Complete(runResult, batchJobExecutionResult);
            return batchJobExecutionResult;
        }
    }
}