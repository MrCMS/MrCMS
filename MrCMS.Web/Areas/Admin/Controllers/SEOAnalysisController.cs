using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Areas.Admin.Services.SEOAnalysis;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SEOAnalysisController : MrCMSAdminController
    {
        private readonly ISEOAnalysisService _seoAnalysisService;

        public SEOAnalysisController(ISEOAnalysisService seoAnalysisService)
        {
            _seoAnalysisService = seoAnalysisService;
        }

        public PartialViewResult Analyze(Webpage webpage, string analysisTerm)
        {
            var result = _seoAnalysisService.Analyze(webpage, analysisTerm);
            return PartialView(result);
        }
    }
}