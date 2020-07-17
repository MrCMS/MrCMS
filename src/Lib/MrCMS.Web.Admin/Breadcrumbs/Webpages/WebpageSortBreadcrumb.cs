using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Webpages
{
    public class WebpageSortBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "Webpage";
        public override string Action => "Sort";
        public override string Name => "Sort";
        public override void Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
        }
    }
}