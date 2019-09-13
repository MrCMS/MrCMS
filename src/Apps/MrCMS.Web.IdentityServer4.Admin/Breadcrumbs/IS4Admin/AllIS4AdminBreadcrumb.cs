using MrCMS.Web.Apps.Admin.Breadcrumbs;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.WebApi.Breadcrumbs.Api
{
    public class AllIS4AdminBreadcrumb : Breadcrumb<IS4AdminBreadcrumb>
    {
      //  public override int Order => 2;
        public override string Controller => "IS4AdminHome";
        public override string Action => "Index";
        public override bool IsNav => true;

        public override string Title => "IS4GRTAS";

        public override string Name => "IS4GRTAS";
    }
}