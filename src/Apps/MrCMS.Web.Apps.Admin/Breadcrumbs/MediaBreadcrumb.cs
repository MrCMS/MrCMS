using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
{
    public class MediaBreadcrumb : Breadcrumb
    {
        public override int Order => 1;
        public override string Controller => "MediaCategory";
        public override string Action => "Index";
        public override bool IsNav => true;
        public override string CssClass => "fa fa-file-image-o";
    }
}