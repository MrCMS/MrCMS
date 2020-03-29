using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Webpages
{
    public class WebpageAddBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "Webpage";
        public override string Action => "Add";
        public override string Name => "Add";
        public override Task Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
            return Task.CompletedTask;
        }
    }
}