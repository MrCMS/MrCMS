using System;
using MrCMS.Services;

namespace MrCMS.Batching.CoreJobs
{
    public class RebuildLuceneIndexExecutor : BaseBatchJobExecutor<RebuildLuceneIndex>
    {
        private readonly IIndexService _indexService;

        public RebuildLuceneIndexExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus, IIndexService indexService)
            : base(setBatchJobJobExecutionStatus)
        {
            _indexService = indexService;
        }

        protected override BatchJobExecutionResult OnExecute(RebuildLuceneIndex batchJob)
        {
            try
            {
                _indexService.Reindex(batchJob.IndexName);
                return BatchJobExecutionResult.Success();
            }
            catch (Exception exception)
            {
                return BatchJobExecutionResult.Success(exception.Message);
            }
        }
    }
}