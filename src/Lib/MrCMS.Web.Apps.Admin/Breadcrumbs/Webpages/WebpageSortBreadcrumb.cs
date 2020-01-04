using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Webpages
{
    public class WebpageSortBreadcrumb : Breadcrumb<WebpageBreadcrumb>
    {
        public override string Controller => "Webpage";
        public override string Action => "Sort";
        public override string Name => "Sort";
        public override Task Populate()
        {
            ParentActionArguments = CreateIdArguments(Id);
        }
    }
}