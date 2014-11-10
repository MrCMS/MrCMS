using Microsoft.AspNet.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Hubs;
using MrCMS.Web.Areas.Admin.Services.Batching;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class UpdateBatchRunResult : IOnUpdated<BatchRunResult>
    {
        private readonly IBatchRunUIService _batchRunUIService;

        public UpdateBatchRunResult(IBatchRunUIService batchRunUIService)
        {
            _batchRunUIService = batchRunUIService;
        }
        public void Execute(OnUpdatedArgs<BatchRunResult> args)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BatchProcessingHub>();
            var batchRunResult = args.Item;
            hubContext.Clients.All.updateResult(batchRunResult.Id);
            var batchRun = batchRunResult.BatchRun;
            hubContext.Clients.All.updateRun(batchRun.ToSimpleJson(_batchRunUIService.GetCompletionStatus(batchRun)));
            hubContext.Clients.All.updateJob(batchRunResult.BatchJob.Id);
        }
    }
}