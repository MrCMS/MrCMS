using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

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