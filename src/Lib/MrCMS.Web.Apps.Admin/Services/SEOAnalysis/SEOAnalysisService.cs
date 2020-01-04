using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;


namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class SEOAnalysisService : ISEOAnalysisService
    {
        private readonly ISession _session;
        private readonly IGetLiveUrl _getLiveUrl;
        private readonly IServiceProvider _serviceProvider;

        public SEOAnalysisService(ISession session, IGetLiveUrl getLiveUrl, IServiceProvider serviceProvider)
        {
            _session = session;
            _getLiveUrl = getLiveUrl;
            _serviceProvider = serviceProvider;
        }

        public SEOAnalysisResult Analyze(Webpage webpage, string analysisTerm)
        {
            HtmlNode htmlNode;
            using (new TemporaryPublisher(_session, webpage))
            {
                htmlNode = GetDocument(webpage);
            }

            var providers = GetProviders();
            return
                new SEOAnalysisResult(
                    providers.SelectMany(provider => provider.GetFacets(webpage, htmlNode, analysisTerm)));
        }

        public Webpage UpdateAnalysisTerm(int webpageId, string targetSeoPhrase)
        {
            var webpage = _session.Get<Webpage>(webpageId);
            if (webpage == null)
                throw new NullReferenceException("Webpage does not exist");
            webpage.SEOTargetPhrase = targetSeoPhrase;
            _session.Transact(session => session.Update(webpage));
            return webpage;
        }

        public IEnumerable<ISEOAnalysisFacetProvider> GetProviders()
        {
            var allConcreteTypesAssignableFrom = TypeHelper.GetAllConcreteTypesAssignableFrom<ISEOAnalysisFacetProvider>();
            return allConcreteTypesAssignableFrom
                .Select(type => _serviceProvider.GetService(type)).Cast<ISEOAnalysisFacetProvider>();
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