using System.Threading.Tasks;
using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class ThirdPartyAuthController : MrCMSAdminController
    {
        private readonly IThirdPartyAuthSettingsAdminService _thirdPartyAuthSettingsAdminService;

        public ThirdPartyAuthController(IThirdPartyAuthSettingsAdminService thirdPartyAuthSettingsAdminService)
        {
            _thirdPartyAuthSettingsAdminService = thirdPartyAuthSettingsAdminService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await _thirdPartyAuthSettingsAdminService.GetSettingsAsync());
        }

        [HttpPost]
        public async Task<RedirectToRouteResult> Index([IoCModelBinder(typeof(ThirdPartyAuthSettingsModelBinder))] ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            await _thirdPartyAuthSettingsAdminService.SaveSettingsAsync(thirdPartyAuthSettings);
            return RedirectToAction("Index");
        }
    }
}