using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Blog.Areas.Admin.Controllers
{
    public class PostController : MrCMSAppAdminController<BlogApp>
    {
        public ViewResult Show()
        {
            return View();
        }
    }
}