using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using NHibernate;

namespace MrCMS.Batching.Services
{
    public class SynchronousBatchRunExecution : ISynchronousBatchRunExecution
    {
        private readonly ISession _session;
        private readonly IExecuteNextBatchJob _executeNextBatchJob;

        public SynchronousBatchRunExecution(ISession session, IExecuteNextBatchJob executeNextBatchJob)
        {
            _session = session;
            _executeNextBatchJob = executeNextBatchJob;
        }

        public async Task Execute(BatchRun run)
        {
            // ensure the run is from the current session
            run = _session.Get<BatchRun>(run.Id);
            run.Status = BatchRunStatus.Executing;
            while (await _executeNextBatchJob.Execute(run))
            {
            }
        }
    }
}