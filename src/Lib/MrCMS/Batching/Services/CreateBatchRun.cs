using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Data;

namespace MrCMS.Batching.Services
{
    public class CreateBatchRun : ICreateBatchRun
    {
        private readonly IRepository<BatchJob> _batchJobRepository;
        private readonly IRepository<BatchRun> _batchRunRepository;
        private readonly IRepository<BatchRunResult> _batchRunResultRepository;

        public CreateBatchRun(
            IRepository<BatchJob> batchJobRepository,
            IRepository<BatchRun> batchRunRepository,
            IRepository<BatchRunResult> batchRunResultRepository)
        {
            _batchJobRepository = batchJobRepository;
            _batchRunRepository = batchRunRepository;
            _batchRunResultRepository = batchRunResultRepository;
        }

        public async Task<BatchRun> Create(Batch batch)
        {
            if (batch == null)
                return null;

            var existingNonFailedResults = _batchRunResultRepository.Readonly().Where(x =>
                    x.BatchJob.BatchId == batch.Id
                    && x.Status != JobExecutionStatus.Failed)
                .Select(x => x.BatchJobId)
                .Distinct()
                .ToList();

            var jobs = _batchJobRepository.Query()
                .Where(job => job.Batch.Id == batch.Id)
                .ToList();

            jobs = jobs.FindAll(j => !existingNonFailedResults.Contains(j.Id));

            var now = DateTime.UtcNow;

            // we need to make sure that the site is loaded from the correct session
            var result = await _batchRunRepository.Add(new BatchRun
            {
                Batch = batch,
                BatchRunResults = new List<BatchRunResult>(),
                SiteId = batch.SiteId,
                CreatedOn = now,
                UpdatedOn = now
            });
            var results = new List<BatchRunResult>();
            var batchRun = result.Model;
            for (int index = 0; index < jobs.Count; index++)
            {
                BatchJob batchJob = jobs[index];
                var batchRunResult = new BatchRunResult
                {
                    BatchJob = batchJob,
                    Status = JobExecutionStatus.Pending,
                    ExecutionOrder = index,
                    BatchRunId = batchRun.Id,
                    SiteId = batch.SiteId,
                    CreatedOn = now,
                    UpdatedOn = now
                };
                results.Add(batchRunResult);
            }

            await _batchRunResultRepository.AddRange(results);

            return batchRun;
        }
    }
}