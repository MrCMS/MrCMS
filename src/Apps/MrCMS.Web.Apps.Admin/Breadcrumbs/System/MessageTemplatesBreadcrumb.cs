using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class MessageTemplatesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 3;
        public override string Controller => "MessageTemplate";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}