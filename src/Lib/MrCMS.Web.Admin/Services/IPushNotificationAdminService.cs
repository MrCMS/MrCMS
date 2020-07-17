using MrCMS.Batching.Entities;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IPushNotificationAdminService
    {
        bool PushToAll(PushNotificationModel model);
    }
}