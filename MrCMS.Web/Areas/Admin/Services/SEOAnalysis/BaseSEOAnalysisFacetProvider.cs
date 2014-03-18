using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public abstract class BaseSEOAnalysisFacetProvider : ISEOAnalysisFacetProvider
    {
        public abstract IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm);

        protected SEOAnalysisFacet GetFacet(string name, SEOAnalysisStatus status, int importance, params string[] messages)
        {
            return new SEOAnalysisFacet
                   {
                       Name = name,
                       Status = status,
                       Messages = messages.ToList(),
                       Importance = importance
                   };
        }
        protected SEOAnalysisFacet GetFacet(string name, SEOAnalysisStatus status, params string[] messages)
        {
            return new SEOAnalysisFacet
                   {
                       Name = name,
                       Status = status,
                       Messages = messages.ToList()
                   };
        }
    }
}