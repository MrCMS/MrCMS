using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Webpages
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