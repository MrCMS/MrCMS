using System.Threading;
using System.Threading.Tasks;
using MrCMS.Services;

namespace MrCMS.Batching.CoreJobs
{
    public class RebuildLuceneIndexExecutor : BaseBatchJobExecutor<RebuildLuceneIndex>
    {
        private readonly IIndexService _indexService;

        public RebuildLuceneIndexExecutor(IIndexService indexService)
        {
            _indexService = indexService;
        }


        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(RebuildLuceneIndex batchJob,
            CancellationToken token)
        {
            await _indexService.Reindex(batchJob.IndexName);
            return BatchJobExecutionResult.Success();
        }
    }
}