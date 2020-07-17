using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class TasksBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override int Order => 10;
        public override string Controller => "Task";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}