using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching.Services
{
    public class SynchronousBatchRunExecution : ISynchronousBatchRunExecution
    {
        private readonly IExecuteNextBatchJob _executeNextBatchJob;

        public SynchronousBatchRunExecution(IExecuteNextBatchJob executeNextBatchJob)
        {
            _executeNextBatchJob = executeNextBatchJob;
        }

        public async Task Execute(BatchRun run)
        {
            while (await _executeNextBatchJob.Execute(run))
            {
            }
        }
    }
}