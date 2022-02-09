using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class TitleIsCorrectLength : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var facets = new List<SEOAnalysisFacet>();
            string titleText = document.GetElementText("title") ?? string.Empty;

            if (titleText.Length < 50)
                facets.Add(GetFacet("Title length", SEOAnalysisStatus.Error,
                    "The title should be at least 50 characters long"));
            else if (titleText.Length > 90)
                facets.Add(
                    GetFacet("Title length", SEOAnalysisStatus.Error,
                        "The title should be at most 90 characters long"));
            else
                facets.Add(
                    GetFacet("Title length", SEOAnalysisStatus.Success,
                        "The title is of optimal length (50-90 characters)"));
            return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(facets);
        }
    }
}