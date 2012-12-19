using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Application.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class TextPageController : MrCMSController
    {
        public ActionResult View(TextPage page)
        {
            return View("~/Views/Pages/TextPage.cshtml", page.CurrentLayout.UrlSegment, page);
        }
    }
}