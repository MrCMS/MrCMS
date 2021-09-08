using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Admin.Services.Batching
{
    public class GetBatchStatus : IGetBatchStatus
    {
        private readonly ISession _session;

        public GetBatchStatus(ISession session)
        {
            _session = session;
        }

        public async Task<BatchStatus> Get(Batch batch)
        {
            if (batch == null)
                return new BatchStatus();
            var anyRuns = await _session.QueryOver<BatchRun>().Where(job => job.Batch.Id == batch.Id).AnyAsync();
            if (!anyRuns)
                return BatchStatus.Pending;
            var anyPaused = await _session.QueryOver<BatchRun>().Where(job => job.Batch.Id == batch.Id && job.Status == BatchRunStatus.Paused).AnyAsync();
            if (anyPaused)
                return BatchStatus.Paused;
            var anyExecuting = await _session.QueryOver<BatchRun>().Where(job => job.Batch.Id == batch.Id && job.Status == BatchRunStatus.Executing).AnyAsync();
            if (anyExecuting)
                return BatchStatus.Executing;
            if (await _session.QueryOver<BatchRun>().Where(job => job.Batch.Id == batch.Id && job.Status != BatchRunStatus.Complete).AnyAsync())
                return BatchStatus.Pending;
            return BatchStatus.Complete;
        }
    }
}