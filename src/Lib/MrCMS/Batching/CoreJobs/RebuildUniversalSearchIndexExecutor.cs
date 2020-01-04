using System;
using System.Threading;
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

        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(RebuildUniversalSearchIndex batchJob,
            CancellationToken token)
        {
            await _universalSearchIndexManager.ReindexAll();
            return BatchJobExecutionResult.Success();
        }
    }
}