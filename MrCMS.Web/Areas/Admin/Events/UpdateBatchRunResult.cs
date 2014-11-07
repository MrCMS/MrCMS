using Microsoft.AspNet.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Areas.Admin.Hubs;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class UpdateBatchRunResult : IOnUpdated<BatchRunResult>
    {
        public void Execute(OnUpdatedArgs<BatchRunResult> args)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BatchProcessingHub>();

            hubContext.Clients.All.updateResult(args.Item.Id);
        }
    }
}