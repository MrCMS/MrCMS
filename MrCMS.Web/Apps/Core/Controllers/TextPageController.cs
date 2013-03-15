using System.Web.Mvc;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class TextPageController : MrCMSUIController
    {
        public ActionResult Show(TextPage page)
        {
            return View("~/Apps/CoreApp/Views/Pages/TextPage.cshtml", page);
        }
    }
}