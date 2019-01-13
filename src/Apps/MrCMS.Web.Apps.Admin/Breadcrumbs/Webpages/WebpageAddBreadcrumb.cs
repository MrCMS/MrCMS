using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Webpages
{
    public class WebpageAddBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "Webpage";
        public override string Action => "Add";
        public override string Name => "Add";
        public override void Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
        }
    }
}