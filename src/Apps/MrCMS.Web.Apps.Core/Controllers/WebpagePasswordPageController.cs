using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Helpers;
using MrCMS.Models.Auth;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class WebpagePasswordPageController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IUnlockPageService _unlockPageService;

        public WebpagePasswordPageController(IUnlockPageService unlockPageService)
        {
            _unlockPageService = unlockPageService;
        }


        [HttpGet]
        [CanonicalLinks]
        public ViewResult Show(WebpagePasswordPage page, int lockedPage)
        {
            ViewData["locked-page"] = _unlockPageService.GetLockedPage(lockedPage);

            return View(page);
        }

        [HttpPost]
        public async Task<RedirectResult> Post(WebpagePasswordPage page, UnlockPageModel model)
        {
            var result = await _unlockPageService.TryUnlockPage(model,Response.Cookies);

            if (result.Success)
            {
                return await _unlockPageService.RedirectToPage(result.LockedPageId);
            }

            TempData["error"] = true;
            return await _unlockPageService.RedirectBackToPage(model);
        }

    }
}