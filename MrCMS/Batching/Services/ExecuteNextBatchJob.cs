using System;
using System.Diagnostics;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Batching.Services
{
    public class ExecuteNextBatchJob : IExecuteNextBatchJob
    {
        private readonly IGetNextJobToRun _getNextJobToRun;
        private readonly IBatchJobExecutionService _batchJobExecutionService;
        private readonly ISetBatchJobExecutionStatus _setBatchJobExecutionStatus;
        private readonly ISetRunStatus _setRunStatus;

        public ExecuteNextBatchJob(IGetNextJobToRun getNextJobToRun, IBatchJobExecutionService batchJobExecutionService,
            ISetBatchJobExecutionStatus setBatchJobExecutionStatus, ISetRunStatus setRunStatus)
        {
            _getNextJobToRun = getNextJobToRun;
            _batchJobExecutionService = batchJobExecutionService;
            _setBatchJobExecutionStatus = setBatchJobExecutionStatus;
            _setRunStatus = setRunStatus;
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

            _setBatchJobExecutionStatus.Starting(runResult);

            var batchJobExecutionResult = _batchJobExecutionService.Execute(runResult.BatchJob);
            
            runResult.MillisecondsTaken = Convert.ToDecimal(stopWatch.Elapsed.TotalMilliseconds);
            _setBatchJobExecutionStatus.Complete(runResult, batchJobExecutionResult);

            return true;
        }
    }
}