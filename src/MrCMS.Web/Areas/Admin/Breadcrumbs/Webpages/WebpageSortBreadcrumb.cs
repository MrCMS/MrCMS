using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Webpages
{
    public class WebpageSortBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "Webpage";
        public override string Action => "Sort";
        public override string Name => "Sort";
        public override Task Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
            return Task.CompletedTask;
        }
    }
}