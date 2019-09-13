using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.WebApi.Breadcrumbs
{
    public class IS4AdminBreadcrumb : Breadcrumb
    {
        public override int Order => 8;
        public override string Controller => "IS4AdminHome";
        public override string Action => "Index";
      //  public override bool IsPlaceHolder => true;
        public override bool IsNav => true;
        public override string CssClass => "fa fa-user-secret";

        public override string Name => "Identity Server";
    }
}