using System;

namespace MrCMS.Web.Admin.Infrastructure.Dashboard
{
    public class DashboardAreaAttribute : Attribute
    {
        public DashboardArea Area { get; set; }
        public int Order { get; set; }
    }
}