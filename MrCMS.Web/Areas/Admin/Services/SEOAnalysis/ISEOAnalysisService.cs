using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public interface ISEOAnalysisService
    {
        SEOAnalysisResult Analyze(Webpage webpage, string analysisTerm);
        void UpdateAnalysisTerm(Webpage webpage);
    }
}