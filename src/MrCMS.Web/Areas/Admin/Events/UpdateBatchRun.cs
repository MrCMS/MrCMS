using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Batching.Entities;
using MrCMS.Data;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Hubs;
using MrCMS.Web.Areas.Admin.Services.Batching;

namespace MrCMS.Web.Areas.Admin.Events
{
    public class UpdateBatchRun : OnDataUpdated<BatchRun>
    {
        private readonly IBatchRunUIService _batchRunUIService;
        private readonly IHubContext<BatchProcessingHub> _context;

        public UpdateBatchRun(IBatchRunUIService batchRunUIService, IHubContext<BatchProcessingHub> context)
        {
            _batchRunUIService = batchRunUIService;
            _context = context;
        }


        public override async Task Execute(ChangeInfo data)
        {
            var batchRun = data.Entity() as BatchRun;
            await _context.Clients.All.SendCoreAsync("updateRun",
                new[]
                {
                    batchRun.ToSimpleJson(_batchRunUIService.GetCompletionStatus(batchRun))
                });
        }
    }
}