using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentNHibernate.Testing.Values;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Mapping;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public class BatchRunUIService : IBatchRunUIService
    {
        private readonly IControlBatchRun _controlBatchRun;
        private readonly IExecuteNextBatchJob _executeNextBatchJob;
        private readonly IExecuteRequestForNextTask _executeRequestForNextTask;
        private readonly ISession _session;

        public BatchRunUIService(ISession session, IControlBatchRun controlBatchRun,
            IExecuteNextBatchJob executeNextBatchJob, IExecuteRequestForNextTask executeRequestForNextTask)
        {
            _session = session;
            _controlBatchRun = controlBatchRun;
            _executeNextBatchJob = executeNextBatchJob;
            _executeRequestForNextTask = executeRequestForNextTask;
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
            return _controlBatchRun.Start(run) ? run.Id : (int?)null;
        }

        public bool Pause(BatchRun run)
        {
            return _controlBatchRun.Pause(run);
        }

        public BatchCompletionStatus GetCompletionStatus(BatchRun batchRun)
        {
            var timeTaken =
                GetResultsQuery(batchRun)
                    .Where(result => result.MillisecondsTaken != null)
                    .Select(Projections.Sum<BatchRunResult>(result => result.MillisecondsTaken))
                    .Cacheable()
                    .FutureValue<decimal?>();
            var averageTimeTaken =
                GetResultsQuery(batchRun)
                    .Where(result => result.MillisecondsTaken != null)
                    .Select(Projections.Avg<BatchRunResult>(result => result.MillisecondsTaken))
                    .Cacheable()
                    .FutureValue<double?>();
            var pending =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Pending)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            var failed =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Failed)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            var succeeded =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Succeeded)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            var total =
                GetResultsQuery(batchRun)
                     .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();


            return new BatchCompletionStatus
            {
                Total = total.Value,
                Failed = failed.Value,
                Pending = pending.Value,
                Succeeded = succeeded.Value,
                TimeTaken = TimeSpan.FromMilliseconds(Convert.ToDouble(timeTaken.Value.GetValueOrDefault())),
                AverageTimeTaken = averageTimeTaken.Value.GetValueOrDefault().ToString("0.00ms")
            };
        }

        public void ExecuteRequestForNextTask(BatchRun run)
        {
            _executeRequestForNextTask.Execute(run);
        }

        public async Task<int?> ExecuteNextTask(BatchRun run)
        {
            return await _executeNextBatchJob.Execute(run) ? run.Id : (int?)null;
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