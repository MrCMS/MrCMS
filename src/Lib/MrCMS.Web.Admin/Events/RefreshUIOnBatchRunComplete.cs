using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Hubs;

namespace MrCMS.Web.Admin.Events
{
    public class RefreshUIOnBatchRunComplete : IOnUpdated<BatchRun>
    {
        private readonly IHubContext<BatchProcessingHub> _context;

        public RefreshUIOnBatchRunComplete(IHubContext<BatchProcessingHub> context)
        {
            _context = context;
        }
        public void Execute(OnUpdatedArgs<BatchRun> args)
        {
            var batchRun = args.Item;
            var previous = args.Original;
            if (batchRun.Status == BatchRunStatus.Complete && previous.Status != BatchRunStatus.Complete)
            {
                _context.Clients.All.SendCoreAsync("refreshBatchRunUI", new object[] { batchRun.Id }).ExecuteSync();
            }
        }
    }
}