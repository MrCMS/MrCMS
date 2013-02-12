using System.Web.Mvc;
using MrCMS.Web.Application.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class TextPageController : MrCMSUIController
    {
        public ActionResult Show(TextPage page)
        {
            return View(page);
        }
    }
}