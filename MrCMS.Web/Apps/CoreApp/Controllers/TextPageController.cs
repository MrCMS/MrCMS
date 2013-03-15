using System.Web.Mvc;
using MrCMS.Web.Apps.CoreApp.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.CoreApp.Controllers
{
    public class TextPageController : MrCMSUIController
    {
        public ActionResult Show(TextPage page)
        {
            return View("~/Apps/CoreApp/Views/Pages/TextPage.cshtml", page);
        }
    }
}