using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Webpages
{
    public class MergeWebpageConfirmBreadcrumb : Breadcrumb<MergeWebpageBreadcrumb>
    {
        public override string Controller => "MergeWebpage";
        public override string Action => "Confirm";
        public override string Name => "Confirm";
        public override Task Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
            return Task.CompletedTask;
        }
    }
}