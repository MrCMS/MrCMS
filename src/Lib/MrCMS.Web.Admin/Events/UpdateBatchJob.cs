using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Admin.Hubs;

namespace MrCMS.Web.Admin.Events
{
    public class UpdateBatchJob : IOnUpdated<BatchJob>
    {
        private readonly IHubContext<BatchProcessingHub> _context;

        public UpdateBatchJob(IHubContext<BatchProcessingHub> context)
        {
            _context = context;
        }
        public async Task Execute(OnUpdatedArgs<BatchJob> args)
        {
            await _context.Clients.All.SendCoreAsync("updateJob", new object[] {args.Item.Id});
        }
    }

}