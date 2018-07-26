using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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