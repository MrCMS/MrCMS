using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Data;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public class GetBatchStatus : IGetBatchStatus
    {
        private readonly IRepository<BatchRun> _repository;

        public GetBatchStatus(IRepository<BatchRun> repository)
        {
            _repository = repository;
        }

        public BatchStatus Get(Batch batch)
        {
            if (batch == null)
                return new BatchStatus();
            var anyRuns = _repository.Readonly().Any(job => job.Batch.Id == batch.Id);
            if (!anyRuns)
                return BatchStatus.Pending;
            var anyPaused = _repository.Readonly().Any(job => job.Batch.Id == batch.Id && job.Status == BatchRunStatus.Paused);
            if (anyPaused)
                return BatchStatus.Paused;
            var anyExecuting = _repository.Readonly().Any(job => job.Batch.Id == batch.Id && job.Status == BatchRunStatus.Executing);
            if (anyExecuting)
                return BatchStatus.Executing;
            if (_repository.Readonly().Any(job => job.Batch.Id == batch.Id && job.Status != BatchRunStatus.Complete))
                return BatchStatus.Pending;
            return BatchStatus.Complete;
        }
    }
}