using System.Collections.Generic;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public interface ISEOAnalysisFacetProvider
    {
        IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm);
    }
}