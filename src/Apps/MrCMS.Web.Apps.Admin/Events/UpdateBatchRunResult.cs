using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Hubs;
using MrCMS.Web.Apps.Admin.Services.Batching;

namespace MrCMS.Web.Apps.Admin.Events
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
        public void Execute(OnUpdatedArgs<BatchRunResult> args)
        {
            var batchRunResult = args.Item;
            _context.Clients.All.SendCoreAsync("updateResult", new object[] { batchRunResult.Id }).ExecuteSync();
            var batchRun = batchRunResult.BatchRun;
            _context.Clients.All.SendCoreAsync("updateRun",
                    new object[] {batchRun.ToSimpleJson(_batchRunUIService.GetCompletionStatus(batchRun))}).ExecuteSync();
            _context.Clients.All.SendCoreAsync("updateJob",
                    new object[] {batchRunResult.BatchJob.Id}).ExecuteSync();
        }
    }
}