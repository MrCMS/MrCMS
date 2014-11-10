using System;
using MrCMS.Search;

namespace MrCMS.Batching.CoreJobs
{
    public class RebuildUniversalSearchIndexExecutor : BaseBatchJobExecutor<RebuildUniversalSearchIndex>
    {
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public RebuildUniversalSearchIndexExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus, IUniversalSearchIndexManager universalSearchIndexManager)
            : base(setBatchJobJobExecutionStatus)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        protected override BatchJobExecutionResult OnExecute(RebuildUniversalSearchIndex batchJob)
        {
            try
            {
                _universalSearchIndexManager.ReindexAll();
                return BatchJobExecutionResult.Success();
            }
            catch (Exception exception)
            {
                return BatchJobExecutionResult.Success(exception.Message);
            }
        }
    }
}