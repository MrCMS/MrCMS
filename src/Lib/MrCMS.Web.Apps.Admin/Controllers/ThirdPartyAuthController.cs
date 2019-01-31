using Microsoft.AspNetCore.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class ThirdPartyAuthController : MrCMSAdminController
    {
        private readonly IThirdPartyAuthSettingsAdminService _thirdPartyAuthSettingsAdminService;

        public ThirdPartyAuthController(IThirdPartyAuthSettingsAdminService thirdPartyAuthSettingsAdminService)
        {
            _thirdPartyAuthSettingsAdminService = thirdPartyAuthSettingsAdminService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_thirdPartyAuthSettingsAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToActionResult Index(
            [ModelBinder(typeof(ThirdPartyAuthSettingsModelBinder))]
            ThirdPartyAuthSettings thirdPartyAuthSettings) 
        {
            _thirdPartyAuthSettingsAdminService.SaveSettings(thirdPartyAuthSettings);
            return RedirectToAction("Index");
        }
    }
}