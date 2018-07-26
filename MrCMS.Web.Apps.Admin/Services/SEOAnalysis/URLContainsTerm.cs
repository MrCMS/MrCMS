using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class URLContainsTerm : BaseSEOAnalysisFacetProvider
    {
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            string url = webpage.AbsoluteUrl;
            yield return
                url.Replace("-", " ").Contains(analysisTerm, StringComparison.OrdinalIgnoreCase)
                    ? GetFacet("URL contains term", SEOAnalysisStatus.Success, "The URL contains '" + analysisTerm + "'")
                    : GetFacet("URL contains term", SEOAnalysisStatus.Error, "The URL does not contain '" + analysisTerm + "'");
        }
    }
}