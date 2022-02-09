using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public abstract class BaseBatchJobExecutor<T> : IBatchJobExecutor where T : BatchJob
    {
        protected abstract Task<BatchJobExecutionResult> OnExecuteAsync(T batchJob);

        public async Task<BatchJobExecutionResult> Execute(BatchJob batchJob)
        {
            var job = batchJob as T;
            if (job == null)
                return BatchJobExecutionResult.Failure("Job is not of type " + typeof(T));
            return await OnExecuteAsync(job);
        }
    }
}