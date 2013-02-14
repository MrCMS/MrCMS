using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class HomeController : MrCMSAdminController
    {
        //
        // GET: /Admin/Home/
        public ActionResult Index()
        {
            return View();
        }
    }
}