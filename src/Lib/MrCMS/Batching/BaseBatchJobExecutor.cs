using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public abstract class BaseBatchJobExecutor<T> : IBatchJobExecutor where T : BatchJob
    {
        private async Task<BatchJobExecutionResult> Execute(T batchJob)
        {
            return await (UseAsync ? OnExecuteAsync(batchJob) : Task.FromResult(OnExecute(batchJob)));
        }

        protected abstract BatchJobExecutionResult OnExecute(T batchJob);


        protected abstract Task<BatchJobExecutionResult> OnExecuteAsync(T batchJob);
        public async Task<BatchJobExecutionResult> Execute(BatchJob batchJob)
        {
            var job = batchJob as T;
            if (job == null)
                return BatchJobExecutionResult.Failure("Job is not of type " + typeof(T));
            return await Execute(job);
        }

        public virtual bool UseAsync { get { return false; } }
    }
}