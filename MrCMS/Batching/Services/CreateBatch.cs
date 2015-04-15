using System;
using System.Collections.Generic;
using MrCMS.Batching.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class CreateBatch : ICreateBatch
    {
        private readonly IStatelessSession _statelessSession;
        private readonly Site _site;
        private readonly ICreateBatchRun _createBatchRun;

        public CreateBatch(IStatelessSession statelessSession, Site site, ICreateBatchRun createBatchRun)
        {
            _statelessSession = statelessSession;
            _site = site;
            _createBatchRun = createBatchRun;
        }

        public BatchCreationResult Create(IEnumerable<BatchJob> jobs)
        {
            DateTime now = CurrentRequestData.Now;
            // we need to make sure that the site is loaded from the correct session
            var site = _statelessSession.Get<Site>(_site.Id);
            var batch = new Batch
            {
                BatchJobs = new List<BatchJob>(),
                BatchRuns = new List<BatchRun>(),
                Site = site,
                CreatedOn = now,
                UpdatedOn = now
            };
            _statelessSession.Transact(session => session.Insert(batch));
            _statelessSession.Transact(session =>
            {
                foreach (BatchJob job in jobs)
                {
                    job.CreatedOn = now;
                    job.UpdatedOn = now;
                    job.Site = site;
                    job.Batch = batch;

                    batch.BatchJobs.Add(job);
                    session.Insert(job);
                }
            });
            BatchRun batchRun = _createBatchRun.Create(batch);
            batch.BatchRuns.Add(batchRun);
            _statelessSession.Transact(session => session.Update(batch));

            return new BatchCreationResult(batch, batchRun);
        }
    }
}