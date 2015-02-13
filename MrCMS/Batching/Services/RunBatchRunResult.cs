using System;
using System.Diagnostics;
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

        public BatchJobExecutionResult Run(BatchRunResult runResult, Stopwatch stopWatch = null)
        {
            stopWatch = stopWatch ?? Stopwatch.StartNew();

            _setBatchJobExecutionStatus.Starting(runResult);

            var batchJobExecutionResult = _batchJobExecutionService.Execute(runResult.BatchJob);

            runResult.MillisecondsTaken = Convert.ToDecimal(stopWatch.Elapsed.TotalMilliseconds);
            _setBatchJobExecutionStatus.Complete(runResult, batchJobExecutionResult);
            return batchJobExecutionResult;
        }
    }
}