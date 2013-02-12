using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Apps.Blog.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Blog.Controllers
{
    public class PostController : MrCMSAppUIController<BlogApp>
    {
        public ActionResult Show(Post page)
        {
            return View("Post", page.CurrentLayout.UrlSegment, page);
        }
    }
}
