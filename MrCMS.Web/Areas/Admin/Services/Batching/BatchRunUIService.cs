using System;
using System.Collections.Generic;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public class BatchRunUIService : IBatchRunUIService
    {
        private readonly IControlBatchRun _controlBatchRun;
        private readonly ISession _session;

        public BatchRunUIService(ISession session, IControlBatchRun controlBatchRun)
        {
            _session = session;
            _controlBatchRun = controlBatchRun;
        }

        public IList<BatchRunResult> GetResults(BatchRun batchRun)
        {
            return GetResultsQuery(batchRun)
                .OrderBy(result => result.ExecutionOrder)
                .Asc.Cacheable()
                .List();
        }

        public int? Start(BatchRun run)
        {
            return _controlBatchRun.Start(run) ? run.Id : (int?) null;
        }

        public bool Pause(BatchRun run)
        {
            return _controlBatchRun.Pause(run);
        }

        public BatchCompletionStatus GetCompletionStatus(BatchRun batchRun)
        {
            IFutureValue<decimal?> timeTaken =
                GetResultsQuery(batchRun)
                    .Where(result => result.MillisecondsTaken != null)
                    .Select(Projections.Sum<BatchRunResult>(result => result.MillisecondsTaken))
                    .Cacheable()
                    .FutureValue<decimal?>();
            IFutureValue<double?> averageTimeTaken =
                GetResultsQuery(batchRun)
                    .Where(result => result.MillisecondsTaken != null)
                    .Select(Projections.Avg<BatchRunResult>(result => result.MillisecondsTaken))
                    .Cacheable()
                    .FutureValue<double?>();
            IFutureValue<int> pending =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Pending)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            IFutureValue<int> failed =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Failed)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            IFutureValue<int> succeeded =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Succeeded)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            IFutureValue<int> total =
                GetResultsQuery(batchRun)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();


            double averageTime = averageTimeTaken.Value.GetValueOrDefault();
            int pendingNumber = pending.Value;
            return new BatchCompletionStatus
            {
                Total = total.Value,
                Failed = failed.Value,
                Pending = pendingNumber,
                Succeeded = succeeded.Value,
                TimeTaken = TimeSpan.FromMilliseconds(Convert.ToDouble(timeTaken.Value.GetValueOrDefault())),
                AverageTimeTaken = averageTime.ToString("0.00ms"),
                EstimatedTimeRemaining = TimeSpan.FromMilliseconds(averageTime*pendingNumber)
            };
        }

        private IQueryOver<BatchRunResult, BatchRunResult> GetResultsQuery(BatchRun batchRun)
        {
            IQueryOver<BatchRunResult, BatchRunResult> queryOver = _session.QueryOver<BatchRunResult>();
            if (batchRun != null)
                return queryOver.Where(result => result.BatchRun.Id == batchRun.Id);
            // query to return 0;
            return queryOver.Where(result => result.Id < 0);
        }
    }
}