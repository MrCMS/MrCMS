using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IPushSubscriptionAdminService 
    {
        Task<IPagedList<PushSubscription>> Search(PushSubscriptionSearchQuery searchQuery);
    }
}