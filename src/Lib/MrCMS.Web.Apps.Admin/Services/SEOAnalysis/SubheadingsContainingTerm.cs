using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class SubheadingsContainingTerm : BaseSEOAnalysisFacetProvider
    {
        public override async IAsyncEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var subheadings = document.GetElementsOfType("h2").ToList();
            if (!subheadings.Any())
            {
                yield return GetFacet("Subheadings", SEOAnalysisStatus.CanBeImproved, "The page contains no subheadings (i.e. h2 tags)");
                yield break;
            }
            var matchingNodes = subheadings.FindAll(node => node.InnerHtml.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase));
            if (!matchingNodes.Any())
                yield return
                    GetFacet("Subheadings", SEOAnalysisStatus.CanBeImproved, string.Format("{0} of the {1} subheadings contain the target term", matchingNodes.Count(), subheadings.Count()));
            else
                yield return
                    GetFacet("Subheadings", SEOAnalysisStatus.Success, string.Format("{0} of the {1} subheadings contain the target term", matchingNodes.Count(), subheadings.Count()));
        }
    }
}