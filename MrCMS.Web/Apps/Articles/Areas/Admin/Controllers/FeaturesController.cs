using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class FeaturesController : MrCMSAppAdminController<ArticlesApp>
    {
        private readonly IFeaturesAdminService _featuresAdminService;

        public FeaturesController(IFeaturesAdminService featuresAdminService)
        {
            _featuresAdminService = featuresAdminService;
        }

        public ViewResult Index(FeaturesSearchQuery query)
        {
            ViewData["feature-section-options"] = _featuresAdminService.GetFeatureSectionOptions();
            ViewData["primary-section-options"] = _featuresAdminService.GetPrimarySectionOptions();
            ViewData["results"] = _featuresAdminService.Search(query);
            return View(query);
        }
    }
}