using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class H1ContainsTerm : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var element = document.GetElement("h1");
            if (element == null)
            {
                return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(new List<SEOAnalysisFacet>
                    {GetFacet("H1 is missing", SEOAnalysisStatus.Error, "The H1 element is missing in the page")});
            }

            string h1Text = document.GetElementText("h1");
            if (h1Text.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(new List<SEOAnalysisFacet>
                {
                    GetFacet("H1 contains term", SEOAnalysisStatus.Success,
                        "The H1 element contains '" + analysisTerm + "'")
                });
            return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(new List<SEOAnalysisFacet>
            {
                GetFacet("H1 contains term", SEOAnalysisStatus.Error,
                    "The H1 element does not contain '" + analysisTerm + "'")
            });
        }
    }
}