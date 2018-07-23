using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Batching
{
    public class BatchExecutionController : MrCMSUIController
    {
        public const string BaseURL = "batch-run/next/";
        private readonly IBatchExecutionService _batchExecutionService;

        public BatchExecutionController(IBatchExecutionService batchExecutionService)
        {
            _batchExecutionService = batchExecutionService;
        }

        [TaskExecutionKeyPasswordAuth]
        public async Task<JsonResult> ExecuteNext(BatchRun run)
        {
            var result = run == null ? null : await _batchExecutionService.ExecuteNextTask(run);
            if (result != null) _batchExecutionService.ExecuteRequestForNextTask(run);
            return Json(result);
        }
    }

    public interface IBatchExecutionService
    {
        Task<int?> ExecuteNextTask(BatchRun run);
        void ExecuteRequestForNextTask(BatchRun run);
    }

    public class BatchExecutionService : IBatchExecutionService
    {
        private readonly IExecuteNextBatchJob _executeNextBatchJob;
        private readonly IExecuteRequestForNextTask _executeRequestForNextTask;

        public BatchExecutionService(IExecuteNextBatchJob executeNextBatchJob,
            IExecuteRequestForNextTask executeRequestForNextTask)
        {
            _executeNextBatchJob = executeNextBatchJob;
            _executeRequestForNextTask = executeRequestForNextTask;
        }

        public void ExecuteRequestForNextTask(BatchRun run)
        {
            _executeRequestForNextTask.Execute(run);
        }

        public async Task<int?> ExecuteNextTask(BatchRun run)
        {
            return await _executeNextBatchJob.Execute(run) ? run.Id : (int?) null;
        }
    }
}