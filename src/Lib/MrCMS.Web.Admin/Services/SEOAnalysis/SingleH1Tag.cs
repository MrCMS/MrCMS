using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class SingleH1Tag : BaseSEOAnalysisFacetProvider
    {
        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var elements = document.GetElementsOfType("h1");
            if (elements.Count() > 1)
                yield return
                    GetFacet("Document contains multiple H1 tags", SEOAnalysisStatus.Error,
                        "There should only be one H1 tag in a document.");
        }
    }
}