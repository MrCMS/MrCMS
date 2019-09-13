using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
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
        [Route(BaseURL)]
        [HttpGet()]
        public async Task<JsonResult> ExecuteNext(Guid? id)
        {
            var result = id == null ? null : await _batchExecutionService.ExecuteNextTask(id.GetValueOrDefault());
            if (result != null) _batchExecutionService.ExecuteRequestForNextTask(id.GetValueOrDefault());
            return Json(result);
        }
    }
}