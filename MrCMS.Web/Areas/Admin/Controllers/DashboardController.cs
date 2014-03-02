using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class DashboardController : MrCMSAdminController
    {
        [DashboardAreaAction(DashboardArea = DashboardArea.Top, Order = -1)]
        public PartialViewResult Greeting()
        {
            return PartialView();
        }
    }
}