using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class TextPageController : MrCMSController
    {
        public ActionResult View(TextPage page)
        {
            return View("~/Views/TextPage/Show.cshtml", page.CurrentLayout.UrlSegment, page);
        }
    }
}