using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.WebApi.Breadcrumbs
{
    public class ApiBreadcrumb : Breadcrumb
    {
        public override int Order => 99;
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;
        public override bool IsNav => true;
        public override string CssClass => "fa fa-database";

        //public override string Title => "APIs";
    }
}