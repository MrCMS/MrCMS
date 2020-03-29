using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Web.Areas.Admin.Hubs;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class UpdateBatchJob : OnDataUpdated<BatchJob>
    {
        private readonly IHubContext<BatchProcessingHub> _context;

        public UpdateBatchJob(IHubContext<BatchProcessingHub> context)
        {
            _context = context;
        }

        public override async Task Execute(ChangeInfo data)
        {
            await _context.Clients.All.SendCoreAsync("updateJob", new object[] {data.EntityId});
        }
    }

}