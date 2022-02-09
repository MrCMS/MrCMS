using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class MessageTemplatesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override decimal Order => 3;
        public override string Controller => "MessageTemplate";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}