using System;
using System.Threading.Tasks;
using MrCMS.Search;

namespace MrCMS.Batching.CoreJobs
{
    public class RebuildUniversalSearchIndexExecutor : BaseBatchJobExecutor<RebuildUniversalSearchIndex>
    {
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public RebuildUniversalSearchIndexExecutor(IUniversalSearchIndexManager universalSearchIndexManager)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        protected override BatchJobExecutionResult OnExecute(RebuildUniversalSearchIndex batchJob)
        {
            _universalSearchIndexManager.ReindexAll();
            return BatchJobExecutionResult.Success();
        }

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(RebuildUniversalSearchIndex batchJob)
        {
            throw new NotImplementedException();
        }
    }
}