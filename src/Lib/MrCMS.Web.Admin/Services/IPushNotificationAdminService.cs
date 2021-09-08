using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IPushNotificationAdminService
    {
        Task<bool> PushToAll(PushNotificationModel model);
        Task<bool> PushToRole(PushToRoleNotificationModel model);
        Task<IPagedList<PushNotification>> Search(PushNotificationSearchModel searchModel);
    }
}