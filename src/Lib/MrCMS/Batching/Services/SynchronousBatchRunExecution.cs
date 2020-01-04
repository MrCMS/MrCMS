using System.Threading;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Data;

namespace MrCMS.Batching.Services
{
    public class SynchronousBatchRunExecution : ISynchronousBatchRunExecution
    {
        private readonly IRepository<BatchRun> _repository;
        private readonly IExecuteNextBatchJob _executeNextBatchJob;

        public SynchronousBatchRunExecution(IRepository<BatchRun> repository, IExecuteNextBatchJob executeNextBatchJob)
        {
            _repository = repository;
            _executeNextBatchJob = executeNextBatchJob;
        }

        public async Task Execute(BatchRun run, CancellationToken token)
        {
            // ensure the run is from the current session
            run = await _repository.Load(run.Id);
            run.Status = BatchRunStatus.Executing;
            while (await _executeNextBatchJob.Execute(run, token))
            {
            }
        }
    }
}