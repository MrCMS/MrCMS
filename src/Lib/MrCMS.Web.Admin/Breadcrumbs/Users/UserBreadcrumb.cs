using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.Users
{
    public class UserBreadcrumb : ItemBreadcrumb<UserListBreadcrumb, User>
    {
        public UserBreadcrumb(ISession session) : base(session)
        {
        }
    }
}