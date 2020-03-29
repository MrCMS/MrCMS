namespace MrCMS.Web.Areas.Admin.Controllers
{
    //public class ThirdPartyAuthController : MrCMSAdminController
    //{
    //    private readonly IThirdPartyAuthSettingsAdminService _thirdPartyAuthSettingsAdminService;

    //    public ThirdPartyAuthController(IThirdPartyAuthSettingsAdminService thirdPartyAuthSettingsAdminService)
    //    {
    //        _thirdPartyAuthSettingsAdminService = thirdPartyAuthSettingsAdminService;
    //    }

    //    [HttpGet]
    //    public ActionResult Index()
    //    {
    //        return View(_thirdPartyAuthSettingsAdminService.GetSettings());
    //    }

    //    [HttpPost]
    //    public RedirectToActionResult Index(
    //        [ModelBinder(typeof(ThirdPartyAuthSettingsModelBinder))]
    //        ThirdPartyAuthSettings thirdPartyAuthSettings) 
    //    {
    //        _thirdPartyAuthSettingsAdminService.SaveSettings(thirdPartyAuthSettings);
    //        return RedirectToAction("Index");
    //    }
    //}
}