using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class DashboardAreaActionAttribute : ActionFilterAttribute
    {
        public DashboardArea DashboardArea { get; set; }
    }
}