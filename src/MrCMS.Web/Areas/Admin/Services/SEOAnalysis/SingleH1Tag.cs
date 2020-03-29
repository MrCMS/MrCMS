using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class SingleH1Tag : BaseSEOAnalysisFacetProvider
    {
        public override async IAsyncEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var elements = document.GetElementsOfType("h1");
            if (Enumerable.Count<HtmlNode>(elements) > 1)
                yield return
           await GetFacet("Document contains multiple H1 tags", SEOAnalysisStatus.Error,
                        "There should only be one H1 tag in a document.");
        }
    }
}