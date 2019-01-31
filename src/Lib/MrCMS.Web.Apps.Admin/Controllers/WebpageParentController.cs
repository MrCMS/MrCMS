using Microsoft.AspNetCore.Mvc;
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
        public PartialViewResult Set(int id)
        {
            var webpage = _webpageParentAdminService.GetWebpage(id);
            ViewData["valid-parents"] = _webpageParentAdminService.GetValidParents(webpage);
            return PartialView(webpage);
        }

        [HttpPost]
        public RedirectToActionResult Set(int id, int? parentVal)
        {
            _webpageParentAdminService.Set(id, parentVal);

            return RedirectToAction("Edit", "Webpage", new { id });
        }
    }
}