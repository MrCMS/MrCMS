using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class URLContainsTerm : BaseSEOAnalysisFacetProvider
    {
        private readonly IGetLiveUrl _getLiveUrl;

        public URLContainsTerm(IGetLiveUrl getLiveUrl)
        {
            _getLiveUrl = getLiveUrl;
        }
        public override async IAsyncEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            string url = await _getLiveUrl.GetAbsoluteUrl(webpage);
            yield return
                url.Replace("-", " ").Contains(analysisTerm, StringComparison.OrdinalIgnoreCase)
                    ? await GetFacet("URL contains term", SEOAnalysisStatus.Success, "The URL contains '" + analysisTerm + "'")
                    : await GetFacet("URL contains term", SEOAnalysisStatus.Error, "The URL does not contain '" + analysisTerm + "'");
        }
    }
}