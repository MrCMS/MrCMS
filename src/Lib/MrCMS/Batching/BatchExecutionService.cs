using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Data;

namespace MrCMS.Batching
{
    public class BatchExecutionService : IBatchExecutionService
    {
        private readonly IRepository<BatchRun> _repository;
        private readonly IExecuteNextBatchJob _executeNextBatchJob;
        private readonly IExecuteRequestForNextTask _executeRequestForNextTask;

        public BatchExecutionService(
            IRepository<BatchRun> repository,
            IExecuteNextBatchJob executeNextBatchJob,
            IExecuteRequestForNextTask executeRequestForNextTask)
        {
            _repository = repository;
            _executeNextBatchJob = executeNextBatchJob;
            _executeRequestForNextTask = executeRequestForNextTask;
        }

        public void ExecuteRequestForNextTask(Guid guid)
        {
            var run = _repository.Query().FirstOrDefault(x => x.Guid == guid);
            _executeRequestForNextTask.Execute(run);
        }

        public async Task<int?> ExecuteNextTask(Guid guid, CancellationToken token)
        {
            var run = _repository.Query().FirstOrDefault(x => x.Guid == guid);
            return await _executeNextBatchJob.Execute(run, token) ? run?.Id : null;
        }
    }
}