using Microsoft.AspNet.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Hubs;
using MrCMS.Web.Areas.Admin.Services.Batching;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class RefreshUIOnBatchRunComplete : IOnUpdated<BatchRun>
    {
        public void Execute(OnUpdatedArgs<BatchRun> args)
        {
            var batchRun = args.Item;
            var previous = args.Original;
            if (batchRun.Status == BatchRunStatus.Complete && previous.Status != BatchRunStatus.Complete)
            {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BatchProcessingHub>();
            hubContext.Clients.All.refreshBatchRunUI(batchRun.Id);
                
            }
        }
    }
    public class UpdateBatchRun : IOnUpdated<BatchRun>
    {
        private readonly IBatchRunUIService _batchRunUIService;

        public UpdateBatchRun(IBatchRunUIService batchRunUIService)
        {
            _batchRunUIService = batchRunUIService;
        }

        public void Execute(OnUpdatedArgs<BatchRun> args)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BatchProcessingHub>();
            var batchRun = args.Item;
            hubContext.Clients.All.updateRun(batchRun.ToSimpleJson(_batchRunUIService.GetCompletionStatus(batchRun)));
            
        }
    }
}