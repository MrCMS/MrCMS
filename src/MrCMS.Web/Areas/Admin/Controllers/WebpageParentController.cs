using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public async Task<RedirectToActionResult> Set(int id, int? parentVal)
        {
            await _webpageParentAdminService.Set(id, parentVal);

            return RedirectToAction("Edit", "Webpage", new { id });
        }
    }
}