using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class BodyContentChecks : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var text =
                (HtmlNode.CreateNode("<div>" + webpage.BodyContent + "</div>").InnerText ?? string.Empty).Replace(
                    Environment.NewLine, " ");

            if (text.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(new List<SEOAnalysisFacet>
                {
                    GetFacet("Body Content", SEOAnalysisStatus.Success,
                        $"Body content contains '{analysisTerm}'")
                });
            }
            else
            {
                return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(new List<SEOAnalysisFacet>
                {
                    GetFacet("Body Content", SEOAnalysisStatus.Error,
                        $"Body content does not contain '{analysisTerm}'")
                });
            }
        }
    }
}