using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class ResourcesBreadcrumb : Breadcrumb<SystemBreadcrumb>
    {
        public override decimal Order => 7;
        public override string Controller => "Resource";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
    
    public class SystemInfo : Breadcrumb<SystemBreadcrumb>
    {
        public override decimal Order => 70;
        public override string Controller => "SystemInfo";
        public override string Action => "Index";
        public override bool IsNav => true;
    }
}