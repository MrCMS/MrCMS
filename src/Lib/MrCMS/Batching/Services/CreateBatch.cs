using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Batching.Services
{
    public class CreateBatch : ICreateBatch
    {
        private readonly IRepository<Batch> _batchRepository;
        private readonly IRepository<BatchJob> _batchJobRepository;
        private readonly ICreateBatchRun _createBatchRun;

        public CreateBatch(IRepository<Batch> batchRepository, IRepository<BatchJob> batchJobRepository, ICreateBatchRun createBatchRun)
        {
            _batchRepository = batchRepository;
            _batchJobRepository = batchJobRepository;
            _createBatchRun = createBatchRun;
        }

        public async Task<BatchCreationResult> Create(IEnumerable<BatchJob> jobs)
        {
            DateTime now = DateTime.UtcNow;
            // we need to make sure that the site is loaded from the correct session
            var batch = new Batch
            {
                BatchJobs = new List<BatchJob>(),
                BatchRuns = new List<BatchRun>(),
                //Site = site,
                CreatedOn = now,
                UpdatedOn = now
            };
            await _batchRepository.Add(batch);
            jobs.ForEach(x => x.Batch = batch);
            await _batchJobRepository.AddRange(jobs.ToList());
            //_statelessSession.Transact(session => session.Insert(batch));
            //_statelessSession.Transact(session =>
            //{
            //    foreach (BatchJob job in jobs)
            //    {
            //        job.CreatedOn = now;
            //        job.UpdatedOn = now;
            //        //job.Site = site;
            //        job.Batch = batch;

            //        batch.BatchJobs.Add(job);
            //        session.Insert(job);
            //    }
            //});
            BatchRun batchRun = await _createBatchRun.Create(batch);
            batch.BatchRuns.Add(batchRun);
            //_statelessSession.Transact(session => session.Update(batch));

            return new BatchCreationResult(batch, batchRun);
        }
    }
}