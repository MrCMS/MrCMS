using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public class DefaultBatchJobExecutor : IBatchJobExecutor
    {
        public BatchJobExecutionResult Execute(BatchJob batchJob)
        {
            return BatchJobExecutionResult.Failure(
                string.Format("There is no executor for this job. To create one, implement {0}<{1}>",
                    typeof (IBatchJobExecutor).FullName,
                    batchJob.GetType().FullName));
        }
    }
}