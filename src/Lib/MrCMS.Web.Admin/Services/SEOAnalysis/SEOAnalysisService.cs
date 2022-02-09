using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using NHibernate;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
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

        public async Task<SEOAnalysisResult> Analyze(Webpage webpage, string analysisTerm)
        {
            HtmlNode htmlNode;
            await using (var publisher = new TemporaryPublisher(_session, webpage))
            {
                await publisher.Publish();
                htmlNode = await GetWebpage(webpage);
            }

            var providers = GetProviders();
            var facets = new List<SEOAnalysisFacet>();
            foreach (var provider in providers)
            {
                facets.AddRange(await provider.GetFacets(webpage, htmlNode, analysisTerm));
            }

            return new SEOAnalysisResult(facets);
        }

        public async Task<Webpage> UpdateAnalysisTerm(int webpageId, string targetSeoPhrase)
        {
            var webpage = _session.Get<Webpage>(webpageId);
            if (webpage == null)
                throw new NullReferenceException("Webpage does not exist");
            webpage.SEOTargetPhrase = targetSeoPhrase;
            await _session.TransactAsync(session => session.UpdateAsync(webpage));
            return webpage;
        }

        public IEnumerable<ISEOAnalysisFacetProvider> GetProviders()
        {
            var allConcreteTypesAssignableFrom =
                TypeHelper.GetAllConcreteTypesAssignableFrom<ISEOAnalysisFacetProvider>();
            return allConcreteTypesAssignableFrom
                .Select(type => _serviceProvider.GetService(type)).Cast<ISEOAnalysisFacetProvider>();
        }
        static readonly HttpClient Client = new HttpClient();

        private async Task<HtmlNode> GetWebpage(Webpage webpage)
        {
            string absoluteUrl = await _getLiveUrl.GetAbsoluteUrl(webpage);
            var request = await Client.GetAsync(absoluteUrl);
            var document = new HtmlDocument();
            document.Load(await request.Content.ReadAsStreamAsync()); 

            HtmlNode htmlNode = document.DocumentNode;
            return htmlNode;
        }

        public class TemporaryPublisher : IAsyncDisposable
        {
            private readonly bool _published;
            private readonly ISession _session;
            private readonly Webpage _webpage;

            public TemporaryPublisher(ISession session, Webpage webpage)
            {
                _session = session;
                _webpage = webpage;
                _published = webpage.Published;
            }

            public async Task Publish()
            {
                if (!_published)
                {
                    _webpage.Published = false;
                    await _session.TransactAsync(s => s.UpdateAsync(_webpage));
                }
            }

            public async ValueTask DisposeAsync()
            {
                if (!_published)
                {
                    _webpage.Published = false;
                    await _session.TransactAsync(s => s.UpdateAsync(_webpage));
                }
            }
        }
    }
}