using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class SEOAnalysisService : ISEOAnalysisService
    {
        private readonly IEnumerable<ISEOAnalysisFacetProvider> _analysisFacetProviders;
        private readonly ISession _session;

        public SEOAnalysisService(IEnumerable<ISEOAnalysisFacetProvider> analysisFacetProviders,ISession session)
        {
            _analysisFacetProviders = analysisFacetProviders;
            _session = session;
        }

        public SEOAnalysisResult Analyze(Webpage webpage, string analysisTerm)
        {
            HtmlNode htmlNode = GetDocument(webpage);
            return new SEOAnalysisResult(_analysisFacetProviders.SelectMany(provider => provider.GetFacets(webpage, htmlNode, analysisTerm)));
        }

        public void UpdateAnalysisTerm(Webpage webpage)
        {
            _session.Transact(session => session.Update(webpage));
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