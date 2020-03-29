using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;
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

        public async Task<PartialViewResult> Analyze(int id, string SEOTargetPhrase)
        {
            var webpage = await _seoAnalysisService.UpdateAnalysisTerm(id, SEOTargetPhrase);
            SEOAnalysisResult result = await _seoAnalysisService.Analyze(webpage, webpage.SEOTargetPhrase);
            return PartialView(result);
        }
    }
}