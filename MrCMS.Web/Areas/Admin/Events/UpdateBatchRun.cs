using Microsoft.AspNet.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Areas.Admin.Hubs;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class UpdateBatchRun : IOnUpdated<BatchRun>
    {
        public void Execute(OnUpdatedArgs<BatchRun> args)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BatchProcessingHub>();
            hubContext.Clients.All.updateRun(args.Item.Id);
        }
    }
}