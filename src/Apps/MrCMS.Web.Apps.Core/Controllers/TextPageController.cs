using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class TextPageController : MrCMSAppUIController<MrCMSCoreApp>
    {
        [CanonicalLinks]
        public ActionResult Show(TextPage page)
        {
            return View(page);
        }
    }
}