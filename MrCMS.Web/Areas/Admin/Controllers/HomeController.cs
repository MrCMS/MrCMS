using System.Web.Mvc;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        //
        // GET: /Admin/Home/
        public ActionResult Index()
        {
            return View();
        }
    }
}