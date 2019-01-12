using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
{
    public class WebpageSortBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        private readonly ISession _session;

        public WebpageSortBreadcrumb(ISession session)
        {
            _session = session;
        }
        public override string Controller => "Webpage";
        public override string Action => "Sort";
        public override string Name => "Sort";
        public override void Populate()
        {
            ParentId = Id;
        }
    }
}