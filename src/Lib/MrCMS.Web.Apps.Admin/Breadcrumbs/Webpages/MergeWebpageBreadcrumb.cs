using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Webpages
{
    public class MergeWebpageBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "MergeWebpage";
        public override string Action => "Index";
        public override string Name => "Merge";
        public override Task Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
            return Task.CompletedTask;
        }
    }
}