using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Users
{
    public class RolesListBreadcrumb : ChildListBreadcrumb<UsersBreadcrumb, UserRole>
    {
        public RolesListBreadcrumb() : base(1)
        {
        }

        public override string Controller => "Role";

        public override string Name => "Roles";
    }
}