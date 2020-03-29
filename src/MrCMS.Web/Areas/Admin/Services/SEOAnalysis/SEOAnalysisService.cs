using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class SEOAnalysisService : ISEOAnalysisService
    {
        private readonly ITemporaryPublisher _temporaryPublisher;
        private readonly IRepository<Webpage> _repository;
        private readonly IGetLiveUrl _getLiveUrl;
        private readonly IServiceProvider _serviceProvider;

        public SEOAnalysisService(ITemporaryPublisher temporaryPublisher, IRepository<Webpage> repository, IGetLiveUrl getLiveUrl, IServiceProvider serviceProvider)
        {
            _temporaryPublisher = temporaryPublisher;
            _repository = repository;
            _getLiveUrl = getLiveUrl;
            _serviceProvider = serviceProvider;
        }

        public async Task<SEOAnalysisResult> Analyze(Webpage webpage, string analysisTerm)
        {
            HtmlNode htmlNode;
            await using (await _temporaryPublisher.TemporarilyPublish(webpage))
            {
                htmlNode = await GetDocument(webpage);
            }

            var providers = GetProviders();
            var facets = new List<SEOAnalysisFacet>();
            foreach (var provider in providers)
            {
                await foreach (var facet in provider.GetFacets(webpage, htmlNode, analysisTerm))
                {
                    facets.Add(facet);
                }
            }

            return
                new SEOAnalysisResult(facets);
        }

        public async Task<Webpage> UpdateAnalysisTerm(int webpageId, string targetSeoPhrase)
        {
            var webpage = await _repository.Load(webpageId);
            if (webpage == null)
                throw new NullReferenceException("Webpage does not exist");
            webpage.SEOTargetPhrase = targetSeoPhrase;
            await _repository.Update(webpage);
            return webpage;
        }

        public IEnumerable<ISEOAnalysisFacetProvider> GetProviders()
        {
            var allConcreteTypesAssignableFrom = TypeHelper.GetAllConcreteTypesAssignableFrom<ISEOAnalysisFacetProvider>();
            return allConcreteTypesAssignableFrom
                .Select(type => _serviceProvider.GetService(type)).Cast<ISEOAnalysisFacetProvider>();
        }

        private async Task<HtmlNode> GetDocument(Webpage webpage)
        {
            string absoluteUrl = await _getLiveUrl.GetAbsoluteUrl(webpage);
            WebRequest request = WebRequest.Create(absoluteUrl);

            var document = new HtmlDocument();
            document.Load(request.GetResponse().GetResponseStream());

            HtmlNode htmlNode = document.DocumentNode;
            return htmlNode;
        }

    }
}