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
        public ActionResult Index()
        {
            return View(_thirdPartyAuthSettingsAdminService.GetSettings());
        }

        [HttpPost]
        public RedirectToRouteResult Index([IoCModelBinder(typeof(ThirdPartyAuthSettingsModelBinder))] ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            _thirdPartyAuthSettingsAdminService.SaveSettings(thirdPartyAuthSettings);
            return RedirectToAction("Index");
        }
    }
}