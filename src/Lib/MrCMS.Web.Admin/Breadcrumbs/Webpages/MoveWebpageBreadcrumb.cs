using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Webpages
{
    public class MoveWebpageBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "MoveWebpage";
        public override string Action => "Index";
        public override string Name => "Move";
        public override void Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
        }
    }
}