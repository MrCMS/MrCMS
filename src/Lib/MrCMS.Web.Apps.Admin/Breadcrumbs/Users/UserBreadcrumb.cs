using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Users
{
    public class UserBreadcrumb : ItemBreadcrumb<UserListBreadcrumb, User>
    {
        public UserBreadcrumb(IGlobalRepository<User> repository) : base(repository)
        {
        }
    }
}