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

        private BatchJobExecutionResult Execute(T batchJob)
        {
            _setBatchJobJobExecutionStatus.Starting(batchJob);
            var result = OnExecute(batchJob);
            _setBatchJobJobExecutionStatus.Complete(batchJob, result);
            return result;
        }

        protected abstract BatchJobExecutionResult OnExecute(T batchJob);

        public BatchJobExecutionResult Execute(BatchJob batchJob)
        {
            var job = batchJob as T;
            return job == null
                ? BatchJobExecutionResult.Failure("Batch job is not of the correct type")
                : Execute(job);
        }
    }
}