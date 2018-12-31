using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Hubs;
using MrCMS.Web.Apps.Admin.Services.Batching;

namespace MrCMS.Web.Apps.Admin.Events
{
    public class UpdateBatchRun : IOnUpdated<BatchRun>
    {
        private readonly IBatchRunUIService _batchRunUIService;
        private readonly IHubContext<BatchProcessingHub> _context;

        public UpdateBatchRun(IBatchRunUIService batchRunUIService, IHubContext<BatchProcessingHub> context)
        {
            _batchRunUIService = batchRunUIService;
            _context = context;
        }

        public void Execute(OnUpdatedArgs<BatchRun> args)
        {
            var batchRun = args.Item;
            _context.Clients.All.SendCoreAsync("updateRun",
                new[]
                {
                    batchRun.ToSimpleJson(_batchRunUIService.GetCompletionStatus(batchRun))
                }).GetAwaiter().GetResult();

        }
    }
}