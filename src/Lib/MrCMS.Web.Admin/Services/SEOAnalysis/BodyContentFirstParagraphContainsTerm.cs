using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class BodyContentFirstParagraphContainsTerm : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            return Task.FromResult(GetFacets(document, analysisTerm));
        }

        private IReadOnlyList<SEOAnalysisFacet> GetFacets(HtmlNode document, string analysisTerm)
        {
            var paragraphs = document.GetElementsOfType("p").ToList();
            if (!paragraphs.Any())
            {
                return new List<SEOAnalysisFacet>
                {
                    GetFacet("Body Content", SEOAnalysisStatus.Error,
                        string.Format("Body content contains no paragraphs of text"))
                };
            }

            if (paragraphs[0].InnerText.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase))
            {
                return new List<SEOAnalysisFacet>
                {
                    GetFacet("Body Content", SEOAnalysisStatus.Success,
                        $"The first paragraph contains the term '{analysisTerm}'")
                };
            }

            return new List<SEOAnalysisFacet>
            {
                GetFacet("Body Content", SEOAnalysisStatus.CanBeImproved,
                    $"The first paragraph does not contain the term '{analysisTerm}'")
            };
        }
    }
}