using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;
using MrCMS.Web.Apps.Admin.Services.SEOAnalysis;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class SEOAnalysisController : MrCMSAdminController
    {
        private readonly ISEOAnalysisService _seoAnalysisService;

        public SEOAnalysisController(ISEOAnalysisService seoAnalysisService)
        {
            _seoAnalysisService = seoAnalysisService;
        }

        public PartialViewResult Analyze(Webpage webpage)
        {
            webpage.SEOTargetPhrase = "test";
            _seoAnalysisService.UpdateAnalysisTerm(webpage);
            SEOAnalysisResult result = _seoAnalysisService.Analyze(webpage, webpage.SEOTargetPhrase);
            return PartialView(result);
        }
    }
}