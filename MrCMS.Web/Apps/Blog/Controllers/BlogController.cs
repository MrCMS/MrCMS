using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Blog.Controllers
{
    public class BlogController : MrCMSUIController
    {
        public BlogController() : base("Blog") { }

        public ActionResult Show(Pages.Blog page)
        {
            return View(page);
        }
    }
}