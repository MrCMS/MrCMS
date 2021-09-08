using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Admin.Services.Batching
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

        public async Task<IList<BatchRunResult>> GetResults(BatchRun batchRun)
        {
            return await GetResultsQuery(batchRun)
                .OrderBy(result => result.ExecutionOrder)
                .Asc.Cacheable()
                .ListAsync();
        }

        public async Task<int?> Start(int id)
        {
            var run = await Get(id);
            return await _controlBatchRun.Start(run) ? run.Id : (int?) null;
        }

        public async Task<bool> Pause(int id)
        {
            var run = await Get(id);
            return await _controlBatchRun.Pause(run);
        }

        public async Task<BatchCompletionStatus> GetCompletionStatus(BatchRun batchRun)
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


            double averageTime = (await averageTimeTaken.GetValueAsync()).GetValueOrDefault();
            int pendingNumber = await pending.GetValueAsync();
            return new BatchCompletionStatus
            {
                Total = await total.GetValueAsync(),
                Failed = await failed.GetValueAsync(),
                Pending = pendingNumber,
                Succeeded = await succeeded.GetValueAsync(),
                TimeTaken = TimeSpan.FromMilliseconds(
                    Convert.ToDouble((await timeTaken.GetValueAsync()).GetValueOrDefault())).ToString("c"),
                AverageTimeTaken = averageTime.ToString("0.00ms"),
                EstimatedTimeRemaining = TimeSpan.FromMilliseconds(averageTime * pendingNumber).ToString("c")
            };
        }

        public async Task<BatchRun> Get(int id)
        {
            return await _session.GetAsync<BatchRun>(id);
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