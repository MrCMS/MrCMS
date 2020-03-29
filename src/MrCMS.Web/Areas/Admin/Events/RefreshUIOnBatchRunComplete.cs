using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Web.Areas.Admin.Hubs;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class RefreshUIOnBatchRunComplete : OnDataUpdated<BatchRun>
    {
        private readonly IHubContext<BatchProcessingHub> _context;

        public RefreshUIOnBatchRunComplete(IHubContext<BatchProcessingHub> context)
        {
            _context = context;
        }

        public override async Task Execute(ChangeInfo data)
        {
            var statusUpdate = data.PropertiesUpdated.FirstOrDefault(x => x.Name == nameof(BatchRun.Status));
            if (statusUpdate != null && statusUpdate.CurrentValue == (object)BatchRunStatus.Complete && statusUpdate.OriginalValue != (object)BatchRunStatus.Complete)
            {
                await _context.Clients.All.SendCoreAsync("refreshBatchRunUI", new object[] {data.EntityId});
            }
        }
    }
}