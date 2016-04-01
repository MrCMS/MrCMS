using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class DashboardAreaAction : ActionFilterAttribute
    {
        public DashboardArea DashboardArea { get; set; }
    }
}