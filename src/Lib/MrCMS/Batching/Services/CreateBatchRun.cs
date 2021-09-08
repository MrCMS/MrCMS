using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Batching.Services
{
    public class CreateBatchRun : ICreateBatchRun
    {
        private readonly IStatelessSession _statelessSession;

        public CreateBatchRun(IStatelessSession statelessSession)
        {
            _statelessSession = statelessSession;
        }

        public async Task<BatchRun> Create(Batch batch)
        {
            if (batch == null)
                return null;
            BatchJob jobAlias = null;

            var subQuery = QueryOver.Of<BatchRunResult>()
                .Where(result => result.Status != JobExecutionStatus.Failed && result.BatchJob.Id == jobAlias.Id)
                .Select(result => result.Id);

            var jobs = _statelessSession.QueryOver(() => jobAlias)
                .Where(job => job.Batch.Id == batch.Id)
                .WithSubquery.WhereNotExists(subQuery)
                .List();

            // we need to make sure that the site is loaded from the correct session
            var site = _statelessSession.Get<Site>(batch.Site.Id);
            return await _statelessSession.TransactAsync(async session =>
            {
                var now = DateTime.UtcNow;
                var batchRun = new BatchRun
                {
                    Batch = batch,
                    BatchRunResults = new List<BatchRunResult>(),
                    Site = site,
                    CreatedOn = now,
                    UpdatedOn = now
                };
                await session.InsertAsync(batchRun);
                for (int index = 0; index < jobs.Count; index++)
                {
                    BatchJob batchJob = jobs[index];
                    var batchRunResult = new BatchRunResult
                    {
                        BatchJob = batchJob,
                        Status = JobExecutionStatus.Pending,
                        ExecutionOrder = index,
                        BatchRun = batchRun,
                        Site = site,
                        CreatedOn = now,
                        UpdatedOn = now
                    };
                    batchRun.BatchRunResults.Add(batchRunResult);
                    await session.InsertAsync(batchRunResult);
                }
                return batchRun;
            });
        }
    }
}