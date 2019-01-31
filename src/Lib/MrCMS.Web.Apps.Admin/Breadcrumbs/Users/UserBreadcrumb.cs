using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Users
{
    public class UserBreadcrumb : ItemBreadcrumb<UserListBreadcrumb, User>
    {
        public UserBreadcrumb(ISession session) : base(session)
        {
        }
    }
}