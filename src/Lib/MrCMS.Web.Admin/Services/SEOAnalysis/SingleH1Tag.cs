using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using MrCMS.Web.Admin.Helpers;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class SingleH1Tag : BaseSEOAnalysisFacetProvider
    {
        public override Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var facets = new List<SEOAnalysisFacet>();
            var elements = document.GetElementsOfType("h1");
            if (elements.Count() > 1)

                facets.Add(GetFacet("Document contains multiple H1 tags", SEOAnalysisStatus.Error,
                    "There should only be one H1 tag in a document."));
            return Task.FromResult<IReadOnlyList<SEOAnalysisFacet>>(facets);
        }
    }
}