using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class WebpageParentController : MrCMSAdminController
    {
        private readonly IWebpageParentAdminService _webpageParentAdminService;

        public WebpageParentController(IWebpageParentAdminService webpageParentAdminService)
        {
            _webpageParentAdminService = webpageParentAdminService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Set(int id)
        {
            var webpage = await _webpageParentAdminService.GetWebpage(id);
            ViewData["valid-parents"] = await _webpageParentAdminService.GetValidParents(webpage);
            return PartialView(webpage);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Set(int id, int? parentVal)
        {
            await _webpageParentAdminService.Set(id, parentVal);

            return RedirectToAction("Edit", "Webpage", new {id});
        }
    }
}