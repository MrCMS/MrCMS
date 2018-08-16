using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class SEOAnalysisService : ISEOAnalysisService
    {
        private readonly IEnumerable<ISEOAnalysisFacetProvider> _analysisFacetProviders;
        private readonly ISession _session;
        private readonly IGetLiveUrl _getLiveUrl;

        public SEOAnalysisService(IEnumerable<ISEOAnalysisFacetProvider> analysisFacetProviders, ISession session, IGetLiveUrl getLiveUrl)
        {
            _analysisFacetProviders = analysisFacetProviders;
            _session = session;
            _getLiveUrl = getLiveUrl;
        }

        public SEOAnalysisResult Analyze(Webpage webpage, string analysisTerm)
        {
            HtmlNode htmlNode;
            using (new TemporaryPublisher(_session, webpage))
            {
                htmlNode = GetDocument(webpage);
            }
            return
                new SEOAnalysisResult(
                    _analysisFacetProviders.SelectMany(provider => provider.GetFacets(webpage, htmlNode, analysisTerm)));
        }

        public void UpdateAnalysisTerm(Webpage webpage)
        {
            _session.Transact(session => session.Update(webpage));
        }

        private HtmlNode GetDocument(Webpage webpage)
        {
            string absoluteUrl = _getLiveUrl.GetAbsoluteUrl(webpage);
            WebRequest request = WebRequest.Create(absoluteUrl);

            var document = new HtmlDocument();
            document.Load(request.GetResponse().GetResponseStream());

            HtmlNode htmlNode = document.DocumentNode;
            return htmlNode;
        }

        public class TemporaryPublisher : IDisposable
        {
            private readonly bool _published;
            private readonly ISession _session;
            private readonly Webpage _webpage;

            public TemporaryPublisher(ISession session, Webpage webpage)
            {
                _session = session;
                _webpage = webpage;
                _published = webpage.Published;
                if (!_published)
                {
                    webpage.Published = true;
                    _session.Transact(s => s.Update(_webpage));
                }
            }

            public void Dispose()
            {
                if (!_published)
                {
                    _webpage.Published = false;
                    _session.Transact(s => s.Update(_webpage));
                }
            }
        }
    }
}