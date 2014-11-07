using Microsoft.AspNet.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Areas.Admin.Hubs;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class UpdateBatchJob : IOnUpdated<BatchJob>
    {
        public void Execute(OnUpdatedArgs<BatchJob> args)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BatchProcessingHub>();
            hubContext.Clients.All.updateJob(args.Item.Id);
        }
    }
}