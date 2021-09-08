using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public interface ISEOAnalysisService
    {
        Task<SEOAnalysisResult> Analyze(Webpage webpage, string analysisTerm);
        Task<Webpage> UpdateAnalysisTerm(int webpageId, string targetSeoPhrase);
    }
}