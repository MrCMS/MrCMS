using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Hubs;
using MrCMS.Web.Apps.Admin.Services.Batching;

namespace MrCMS.Web.Apps.Admin.Events
{
    public class UpdateBatchRunResult : OnDataUpdated<BatchRunResult>
    {
        private readonly IBatchRunUIService _batchRunUIService;
        private readonly IHubContext<BatchProcessingHub> _context;

        public UpdateBatchRunResult(IBatchRunUIService batchRunUIService, IHubContext<BatchProcessingHub> context)
        {
            _batchRunUIService = batchRunUIService;
            _context = context;
        }

        public override async Task Execute(ChangeInfo data)
        {
            var batchRunResult = data.Entity() as BatchRunResult;
            await _context.Clients.All.SendCoreAsync("updateResult", new object[] { batchRunResult.Id });
            var batchRun = batchRunResult.BatchRun;
            await _context.Clients.All.SendCoreAsync("updateRun", new object[] { batchRun.ToSimpleJson(_batchRunUIService.GetCompletionStatus(batchRun)) });
            await _context.Clients.All.SendCoreAsync("updateJob", new object[] { batchRunResult.BatchJob.Id });
        }
    }
}