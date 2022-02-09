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
    public class SubheadingsContainingTerm : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var facets = new List<SEOAnalysisFacet>();
            var subheadings = document.GetElementsOfType("h2").ToList();
            if (!subheadings.Any())
            {
                facets.Add(GetFacet("Subheadings", SEOAnalysisStatus.CanBeImproved,
                    "The page contains no subheadings (i.e. h2 tags)"));
                return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(facets);
            }

            var matchingNodes = subheadings.FindAll(node =>
                node.InnerHtml.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase));
            if (!matchingNodes.Any())
                facets.Add(GetFacet("Subheadings", SEOAnalysisStatus.CanBeImproved,
                    $"{matchingNodes.Count()} of the {subheadings.Count()} subheadings contain the target term")
                );
            else
                facets.Add(
                    GetFacet("Subheadings", SEOAnalysisStatus.Success,
                        $"{matchingNodes.Count()} of the {subheadings.Count()} subheadings contain the target term")
                );
            return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(facets);
        }
    }
}