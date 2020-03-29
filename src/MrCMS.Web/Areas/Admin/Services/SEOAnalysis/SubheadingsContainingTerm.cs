using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class SubheadingsContainingTerm : BaseSEOAnalysisFacetProvider
    {
        public override async IAsyncEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var subheadings = Enumerable.ToList<HtmlNode>(document.GetElementsOfType("h2"));
            if (!subheadings.Any())
            {
                yield return await GetFacet("Subheadings", SEOAnalysisStatus.CanBeImproved, "The page contains no subheadings (i.e. h2 tags)");
                yield break;
            }
            var matchingNodes = subheadings.FindAll(node => node.InnerHtml.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase));
            if (!matchingNodes.Any())
                yield return
                    await GetFacet("Subheadings", SEOAnalysisStatus.CanBeImproved, string.Format("{0} of the {1} subheadings contain the target term", matchingNodes.Count(), subheadings.Count()));
            else
                yield return
                    await GetFacet("Subheadings", SEOAnalysisStatus.Success, string.Format("{0} of the {1} subheadings contain the target term", matchingNodes.Count(), subheadings.Count()));
        }
    }
}