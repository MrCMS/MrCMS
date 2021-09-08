using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Users
{
    public class PushSubscriptionsBreadcrumb : ChildListBreadcrumb<UsersBreadcrumb, PushSubscription>
    {
        public PushSubscriptionsBreadcrumb() : base(10)
        {
        }

        public override string Name => "Push Subscriptions";
    }
}