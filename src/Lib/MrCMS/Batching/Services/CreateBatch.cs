using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class CreateBatch : ICreateBatch
    {
        private readonly IStatelessSession _statelessSession;
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly ICreateBatchRun _createBatchRun;

        public CreateBatch(IStatelessSession statelessSession, ICurrentSiteLocator siteLocator,
            ICreateBatchRun createBatchRun)
        {
            _statelessSession = statelessSession;
            _siteLocator = siteLocator;
            _createBatchRun = createBatchRun;
        }

        public async Task<BatchCreationResult> Create(IEnumerable<BatchJob> jobs)
        {
            DateTime now = DateTime.UtcNow;
            // we need to make sure that the site is loaded from the correct session
            var currentSite = _siteLocator.GetCurrentSite();
            var site = _statelessSession.Get<Site>(currentSite.Id);
            var batch = new Batch
            {
                BatchJobs = new List<BatchJob>(),
                BatchRuns = new List<BatchRun>(),
                Site = site,
                CreatedOn = now,
                UpdatedOn = now
            };
            await _statelessSession.TransactAsync(session => session.InsertAsync(batch));
            await _statelessSession.TransactAsync(async session =>
            {
                foreach (BatchJob job in jobs)
                {
                    job.CreatedOn = now;
                    job.UpdatedOn = now;
                    job.Site = site;
                    job.Batch = batch;

                    batch.BatchJobs.Add(job);
                    await session.InsertAsync(job);
                }
            });
            BatchRun batchRun = await _createBatchRun.Create(batch);
            batch.BatchRuns.Add(batchRun);
            await _statelessSession.TransactAsync(session => session.UpdateAsync(batch));

            return new BatchCreationResult(batch, batchRun);
        }
    }
}