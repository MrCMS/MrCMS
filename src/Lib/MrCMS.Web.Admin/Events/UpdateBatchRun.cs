using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Admin.Hubs;
using MrCMS.Web.Admin.Services.Batching;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Events
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

        public async Task Execute(OnUpdatedArgs<BatchRun> args)
        {
            var batchRun = args.Item;
            await _context.Clients.All.SendCoreAsync("updateRun",
                new[]
                {
                    batchRun.ToSimpleJson(await _batchRunUIService.GetCompletionStatus(batchRun))
                });

        }
    }
}