using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Webpages
{
    public class MoveWebpageBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "MoveWebpage";
        public override string Action => "Index";
        public override string Name => "Move";
        public override Task Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
            return Task.CompletedTask;
        }
    }
}