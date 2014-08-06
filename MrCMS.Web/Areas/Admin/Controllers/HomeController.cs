using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class HomeController : MrCMSAdminController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}