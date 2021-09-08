using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Redirects
{
    public class AllRedirectsBreadcrumb : Breadcrumb<RedirectsBreadcrumb>
    {
        private const string BreadcrumbName = "All Redirects";

        public AllRedirectsBreadcrumb()
        {
            Name = BreadcrumbName;
        }
        public override decimal Order => 0;
        public override string Controller => "Redirects";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}