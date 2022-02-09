using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.Webpages
{
    public class MoveWebpageConfirmBreadcrumb : Breadcrumb<MoveWebpageBreadcrumb>
    {
        public override string Controller => "MoveWebpage";
        public override string Action => "Confirm";
        public override string Name => "Confirm";
        public override void Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
        }
    }
}