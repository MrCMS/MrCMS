using MrCMS.Batching.Entities;

namespace MrCMS.Batching
{
    public class BatchCreationResult
    {
        public BatchCreationResult(Batch batch, BatchRun initialBatchRun)
        {
            Batch = batch;
            InitialBatchRun = initialBatchRun;
        }

        public Batch Batch { get; private set; }
        public BatchRun InitialBatchRun { get; private set; }
    }
}