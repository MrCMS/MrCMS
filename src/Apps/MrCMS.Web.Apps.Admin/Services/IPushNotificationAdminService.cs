using MrCMS.Batching.Entities;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IPushNotificationAdminService
    {
        bool PushToAll(PushNotificationModel model);
    }
}