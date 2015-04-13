using System.Threading.Tasks;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public abstract class BaseBatchJobExecutor<T> : IBatchJobExecutor where T : BatchJob
    {
        private readonly ISetBatchJobExecutionStatus _setBatchJobJobExecutionStatus;

        protected BaseBatchJobExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus)
        {
            _setBatchJobJobExecutionStatus = setBatchJobJobExecutionStatus;
        }

        private async Task<BatchJobExecutionResult> Execute(T batchJob)
        {
            _setBatchJobJobExecutionStatus.Starting(batchJob);
            var result = await (UseAsync ? OnExecuteAsync(batchJob) : Task.FromResult(OnExecute(batchJob)));
            _setBatchJobJobExecutionStatus.Complete(batchJob, result);
            return result;
        }

        protected abstract BatchJobExecutionResult OnExecute(T batchJob);


        protected abstract Task<BatchJobExecutionResult> OnExecuteAsync(T batchJob);
        public async Task<BatchJobExecutionResult> Execute(BatchJob batchJob)
        {
            return await BatchJobExecutionResult.TryAsync(() =>
            {
                var job = batchJob as T;
                return job == null
                    ? Task.FromResult(BatchJobExecutionResult.Failure("Batch job is not of the correct type"))
                    : Execute(job);
            });
        }

        public virtual bool UseAsync { get { return false; } }
    }
}