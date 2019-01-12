using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
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