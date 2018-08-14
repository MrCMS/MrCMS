using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Apps.Admin.Hubs;

namespace MrCMS.Web.Apps.Admin.Events
{
    public class UpdateBatchJob : IOnUpdated<BatchJob>
    {
        private readonly IHubContext<BatchProcessingHub> _context;

        public UpdateBatchJob(IHubContext<BatchProcessingHub> context)
        {
            _context = context;
        }
        public void Execute(OnUpdatedArgs<BatchJob> args)
        {
            _context.Clients.All.SendCoreAsync("updateJob", new object[] { args.Item.Id }).GetAwaiter().GetResult();
        }
    }

}