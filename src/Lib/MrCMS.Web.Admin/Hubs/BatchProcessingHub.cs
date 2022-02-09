using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MrCMS.Web.Admin.Hubs
{
    [Authorize("admin")]
    public class BatchProcessingHub : Hub
    {
    }
}