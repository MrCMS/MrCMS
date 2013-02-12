using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Blog.Controllers
{
    public class BlogController : MrCMSAppUIController<BlogApp>
    {
        public ActionResult Show(Pages.Blog page)
        {
            return View(page);
        }
    }
}