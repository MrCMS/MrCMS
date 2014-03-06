using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class SEOAnalysisService : ISEOAnalysisService
    {
        private readonly IEnumerable<ISEOAnalysisFacetProvider> _analysisFacetProviders;

        public SEOAnalysisService(IEnumerable<ISEOAnalysisFacetProvider> analysisFacetProviders)
        {
            _analysisFacetProviders = analysisFacetProviders;
        }

        public SEOAnalysisResult Analyze(Webpage webpage, string analysisTerm)
        {
            HtmlNode htmlNode = GetDocument(webpage);
            return new SEOAnalysisResult(_analysisFacetProviders.SelectMany(provider => provider.GetFacets(webpage, htmlNode, analysisTerm)));
        }

        private static HtmlNode GetDocument(Webpage webpage)
        {
            string absoluteUrl = webpage.AbsoluteUrl;
            WebRequest request = WebRequest.Create(absoluteUrl);

            var document = new HtmlDocument();
            document.Load(request.GetResponse().GetResponseStream());

            var htmlNode = document.DocumentNode;
            return htmlNode;
        }
    }
}