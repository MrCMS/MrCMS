using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MrCMS.Web.Areas.Admin.Hubs
{
    [Authorize("admin")]
    public class BatchProcessingHub : Hub
    {
    }
}