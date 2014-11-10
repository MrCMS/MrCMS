using System.Collections.Generic;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Batching.Services
{
    public class CreateBatchRun : ICreateBatchRun
    {
        private readonly ISession _session;

        public CreateBatchRun(ISession session)
        {
            _session = session;
        }

        public BatchRun Create(Batch batch)
        {
            if (batch == null)
                return null;
            BatchJob jobAlias = null;

            var subQuery = QueryOver.Of<BatchRunResult>()
                .Where(result => result.Status != JobExecutionStatus.Failed && result.BatchJob.Id == jobAlias.Id)
                .Select(result => result.Id);

            var jobs = _session.QueryOver(() => jobAlias)
                .Where(job => job.Batch.Id == batch.Id)
                .WithSubquery.WhereNotExists(subQuery)
                .List();

            return _session.Transact(session =>
            {
                var batchRun = new BatchRun {Batch = batch, BatchRunResults = new List<BatchRunResult>()};
                session.Save(batchRun);
                for (int index = 0; index < jobs.Count; index++)
                {
                    BatchJob batchJob = jobs[index];
                    var batchRunResult = new BatchRunResult
                    {
                        BatchJob = batchJob,
                        Status = JobExecutionStatus.Pending,
                        ExecutionOrder = index,
                        BatchRun = batchRun
                    };
                    batchRun.BatchRunResults.Add(batchRunResult);
                    session.Save(batchRunResult);
                }
                return batchRun;
            });
        }
    }
}