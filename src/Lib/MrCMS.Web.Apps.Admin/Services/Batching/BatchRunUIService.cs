using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Data;


namespace MrCMS.Web.Apps.Admin.Services.Batching
{
    public class BatchRunUIService : IBatchRunUIService
    {
        private readonly IRepository<BatchRun> _repository;
        private readonly IRepository<BatchRunResult> _batchRunResultRepository;

        private readonly IControlBatchRun _controlBatchRun;

        public BatchRunUIService(IRepository<BatchRun> repository, IRepository<BatchRunResult> batchRunResultRepository, IControlBatchRun controlBatchRun)
        {
            _repository = repository;
            _batchRunResultRepository = batchRunResultRepository;
            _controlBatchRun = controlBatchRun;
        }

        public IList<BatchRunResult> GetResults(BatchRun batchRun)
        {
            return GetResultsQuery(batchRun)
                .OrderBy(result => result.ExecutionOrder)
                .ToList();
        }

        public async Task<int?> Start(int id)
        {
            var run = await _repository.Load(id);
            return await _controlBatchRun.Start(run) ? run.Id : (int?)null;
        }

        public async Task<bool> Pause(int id)
        {
            var run = await _repository.Load(id);
            return await _controlBatchRun.Pause(run);
        }

        public BatchCompletionStatus GetCompletionStatus(BatchRun batchRun)
        {
            var timeTaken =
                    GetResultsQuery(batchRun)
                        .Where(result => result.MillisecondsTaken != null)
                        .Sum(result => result.MillisecondsTaken)
                ;
            var averageTimeTaken =
                GetResultsQuery(batchRun)
                    .Where(result => result.MillisecondsTaken != null)
                    .Average(result => result.MillisecondsTaken);
            var pending =
                    GetResultsQuery(batchRun)
                        .Count(result => result.Status == JobExecutionStatus.Pending)
                ;
            var failed =
                GetResultsQuery(batchRun)
                    .Count(result => result.Status == JobExecutionStatus.Failed);

            var succeeded =
                GetResultsQuery(batchRun)
                    .Count(result => result.Status == JobExecutionStatus.Succeeded);

            var total =
                GetResultsQuery(batchRun)
                    .Count();



            decimal averageTime = averageTimeTaken.GetValueOrDefault();
            int pendingNumber = pending;
            return new BatchCompletionStatus
            {
                Total = total,
                Failed = failed,
                Pending = pendingNumber,
                Succeeded = succeeded,
                TimeTaken = TimeSpan.FromMilliseconds(Convert.ToDouble(timeTaken.GetValueOrDefault())),
                AverageTimeTaken = averageTime.ToString("0.00ms"),
                EstimatedTimeRemaining = TimeSpan.FromMilliseconds(Convert.ToDouble(averageTime * pendingNumber))
            };
        }

        private IQueryable<BatchRunResult> GetResultsQuery(BatchRun batchRun)
        {
            var queryOver = _batchRunResultRepository.Query();
            if (batchRun != null)
                return queryOver.Where(result => result.BatchRunId == batchRun.Id);
            // query to return 0;
            return queryOver.Where(result => result.Id < 0);
        }
    }
}