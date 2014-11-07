using MrCMS.Batching.Entities;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class GetNextJobToRun : IGetNextJobToRun
    {
        private readonly ISession _session;

        public GetNextJobToRun(ISession session)
        {
            _session = session;
        }

        public NextJobToRunResult Get(BatchRun batchRun)
        {
            if (batchRun.Status == BatchRunStatus.Pending)
                return new NextJobToRunResult();
            if (batchRun.Status == BatchRunStatus.Complete)
                return new NextJobToRunResult { Complete = true };
            if (batchRun.Status == BatchRunStatus.Paused)
                return new NextJobToRunResult { Paused = true };

            var runResult = _session.QueryOver<BatchRunResult>()
                .Where(result => result.Status == JobExecutionStatus.Pending)
                .OrderBy(result => result.ExecutionOrder).Asc
                .Take(1).SingleOrDefault();
            return runResult == null ? new NextJobToRunResult { Complete = true } : new NextJobToRunResult { Result = runResult };
        }
    }
}