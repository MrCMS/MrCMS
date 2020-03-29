using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Users
{
    public class RolesListBreadcrumb : ChildListBreadcrumb<UsersBreadcrumb, Role>
    {
        public RolesListBreadcrumb() : base(1)
        {
        }

        public override string Controller => "Role";

        public override string Name => "Roles";
    }
}