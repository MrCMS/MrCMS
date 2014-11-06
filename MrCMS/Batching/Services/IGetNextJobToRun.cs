using MrCMS.Batching.Entities;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public interface IGetNextJobToRun
    {
        BatchRunResult Get(BatchRun batchRun);
    }

    public class GetNextJobToRun : IGetNextJobToRun
    {
        private readonly ISession _session;

        public GetNextJobToRun(ISession session)
        {
            _session = session;
        }

        public BatchRunResult Get(BatchRun batchRun)
        {
            if (batchRun.Status != BatchRunStatus.Executing)
                return null;

            var runResult = _session.QueryOver<BatchRunResult>()
                .Where(result => result.Status == JobExecutionStatus.Pending)
                .OrderBy(result => result.ExecutionOrder).Asc
                .Take(1).SingleOrDefault();

            return runResult;
        }
    }
}