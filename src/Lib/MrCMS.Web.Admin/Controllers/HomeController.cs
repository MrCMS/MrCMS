using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class HomeController : MrCMSAdminController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}