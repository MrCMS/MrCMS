using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MrCMS.Web.Areas.Admin.Hubs
{
    [MrCMSAuthorize]
    [HubName("batch")]
    public class BatchProcessingHub : Hub
    {
        
    }
}