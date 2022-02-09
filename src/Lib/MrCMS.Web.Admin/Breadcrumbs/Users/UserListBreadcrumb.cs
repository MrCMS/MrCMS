using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Users
{
    public class UserListBreadcrumb : ChildListBreadcrumb<UsersBreadcrumb, User>
    {
        public UserListBreadcrumb() : base(0)
        {
        }

        public override string Name => "Users";
    }
}