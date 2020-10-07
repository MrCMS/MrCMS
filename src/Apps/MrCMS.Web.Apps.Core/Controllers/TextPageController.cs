using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class TextPageController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IWebpageUIService _uiService;

        public TextPageController(IWebpageUIService uiService)
        {
            _uiService = uiService;
        }

        [CanonicalLinks]
        public ActionResult Show(int id)
        {
            var page = _uiService.GetPage<TextPage>(id);
            return View(page);
        }
    }
}