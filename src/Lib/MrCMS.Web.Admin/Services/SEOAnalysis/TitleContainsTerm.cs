using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class TitleContainsTerm : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var facets = new List<SEOAnalysisFacet>();
            string titleText = document.GetElementText("title");
            if (titleText != null && titleText.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase))
            {
                if (titleText.StartsWith(analysisTerm))
                    facets.Add(
                        GetFacet("Title contains term", SEOAnalysisStatus.Success,
                            $"The title starts with the term '{analysisTerm}', which is considered to improve rankings")
                    );
                else
                {
                    facets.Add(
                        GetFacet("Title contains term", SEOAnalysisStatus.CanBeImproved,
                            $"The title contains the term '{analysisTerm}'. To improve this, consider moving it to the start of the title, as this is considered to improve rankings"));
                }
            }
            else
                facets.Add(
                    GetFacet("Title contains term", SEOAnalysisStatus.Error,
                        $"The title does not contain '{analysisTerm}'"));
            return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(facets);
        }
    }
}