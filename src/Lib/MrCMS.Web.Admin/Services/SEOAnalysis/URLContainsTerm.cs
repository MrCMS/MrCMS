using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class URLContainsTerm : BaseSEOAnalysisFacetProvider
    {
        private readonly IGetLiveUrl _getLiveUrl;

        public URLContainsTerm(IGetLiveUrl getLiveUrl)
        {
            _getLiveUrl = getLiveUrl;
        }
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            string url = _getLiveUrl.GetAbsoluteUrl(webpage);
            yield return
                url.Replace("-", " ").Contains(analysisTerm, StringComparison.OrdinalIgnoreCase)
                    ? GetFacet("URL contains term", SEOAnalysisStatus.Success, "The URL contains '" + analysisTerm + "'")
                    : GetFacet("URL contains term", SEOAnalysisStatus.Error, "The URL does not contain '" + analysisTerm + "'");
        }
    }
}