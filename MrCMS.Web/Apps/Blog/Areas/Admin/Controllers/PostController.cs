using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Blog.Areas.Admin.Controllers
{
    public class PostController : AdminController
    {
        public ViewResult Show()
        {
            return View();
        }
    }
}