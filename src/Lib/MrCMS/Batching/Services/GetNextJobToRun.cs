using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Data;

namespace MrCMS.Batching.Services
{
    public class GetNextJobToRun : IGetNextJobToRun
    {
        private readonly IRepository<BatchRunResult> _repository;
        //private readonly ISession _session;

        public GetNextJobToRun(IRepository<BatchRunResult> repository)
        {
            _repository = repository;
        }

        public NextJobToRunResult Get(BatchRun batchRun)
        {
            if (batchRun.Status == BatchRunStatus.Pending)
                return new NextJobToRunResult();
            if (batchRun.Status == BatchRunStatus.Complete)
                return new NextJobToRunResult { Complete = true };
            if (batchRun.Status == BatchRunStatus.Paused)
                return new NextJobToRunResult { Paused = true };

            var runResult = _repository.Query()
                .Where(result => result.Status == JobExecutionStatus.Pending && result.BatchRun.Id == batchRun.Id)
                .OrderBy(result => result.ExecutionOrder)
                .Take(1).SingleOrDefault();
            return runResult == null ? new NextJobToRunResult { Complete = true } : new NextJobToRunResult { Result = runResult };
        }
    }
}