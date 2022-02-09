using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Web.Admin.Hubs;
using MrCMS.Web.Admin.Services.Batching;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Events
{
    public class UpdateBatchRunResult : IOnUpdated<BatchRunResult>
    {
        private readonly IBatchRunUIService _batchRunUIService;
        private readonly IHubContext<BatchProcessingHub> _context;

        public UpdateBatchRunResult(IBatchRunUIService batchRunUIService, IHubContext<BatchProcessingHub> context)
        {
            _batchRunUIService = batchRunUIService;
            _context = context;
        }

        public async Task Execute(OnUpdatedArgs<BatchRunResult> args)
        {
            var batchRunResult = args.Item;
            await _context.Clients.All.SendCoreAsync("updateResult", new object[] {batchRunResult.Id});
            var batchRun = batchRunResult.BatchRun;
            await _context.Clients.All.SendCoreAsync("updateRun",
                new object[] {batchRun.ToSimpleJson(await _batchRunUIService.GetCompletionStatus(batchRun))});
            await _context.Clients.All.SendCoreAsync("updateJob",
                new object[] {batchRunResult.BatchJob.Id});
        }
    }
}