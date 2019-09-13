using MrCMS.Web.Apps.Admin.Breadcrumbs;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.WebApi.Breadcrumbs.Api
{
    public class AllApiBreadcrumb : Breadcrumb<ApiBreadcrumb>
    {
      //  public override int Order => 2;
        public override string Controller => "Documentation";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}