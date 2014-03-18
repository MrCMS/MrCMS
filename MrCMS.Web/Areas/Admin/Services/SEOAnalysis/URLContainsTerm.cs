using System;
using System.Collections.Generic;
using System.Web.WebPages;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class URLContainsTerm : BaseSEOAnalysisFacetProvider
    {
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            string url = webpage.AbsoluteUrl;
            yield return
                url.Contains(analysisTerm, StringComparison.OrdinalIgnoreCase)
                    ? GetFacet("URL contains term", SEOAnalysisStatus.Success, "The URL contains '" + analysisTerm + "'")
                    : GetFacet("URL contains term", SEOAnalysisStatus.Error, "The URL does not contain '" + analysisTerm + "'");
        }
    }
}