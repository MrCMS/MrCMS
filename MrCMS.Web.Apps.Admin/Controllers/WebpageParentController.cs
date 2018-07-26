using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class WebpageParentController : MrCMSAdminController
    {
        private readonly IWebpageParentAdminService _webpageParentAdminService;

        public WebpageParentController(IWebpageParentAdminService webpageParentAdminService)
        {
            _webpageParentAdminService = webpageParentAdminService;
        }

        [HttpGet]
        public PartialViewResult Set(Webpage webpage)
        {
            ViewData["valid-parents"] = _webpageParentAdminService.GetValidParents(webpage);
            return PartialView(webpage);
        }

        [HttpPost]
        public RedirectToActionResult Set(Webpage webpage, int? parentVal)
        {
            _webpageParentAdminService.Set(webpage, parentVal);

            return RedirectToAction("Edit", "Webpage", new { id = webpage.Id });
        }
    }
}