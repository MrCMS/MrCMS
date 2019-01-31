using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public interface ISEOAnalysisService
    {
        SEOAnalysisResult Analyze(Webpage webpage, string analysisTerm);
        Webpage UpdateAnalysisTerm(int webpageId, string targetSeoPhrase);
    }
}