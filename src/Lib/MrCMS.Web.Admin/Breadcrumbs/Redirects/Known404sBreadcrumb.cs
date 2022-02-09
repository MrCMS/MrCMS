using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Redirects
{
    public class Known404sBreadcrumb : Breadcrumb<RedirectsBreadcrumb>
    {
        private const string BreadcrumbName = "Known 404s";

        public Known404sBreadcrumb()
        {
            Name = BreadcrumbName;
        }
        public override decimal Order => 10;
        public override string Controller => "Redirects";
        public override string Action => "Known404s";
        public override bool IsNav => true;
    }
}