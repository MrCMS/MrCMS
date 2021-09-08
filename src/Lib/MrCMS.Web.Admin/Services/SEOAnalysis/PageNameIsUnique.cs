using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using NHibernate;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class PageNameIsUnique : BaseSEOAnalysisFacetProvider
    {
        private readonly ISession _session;

        public PageNameIsUnique(ISession session)
        {
            _session = session;
        }

        public override async Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            bool anyWithSameTitle =
                await _session.QueryOver<Webpage>()
                    .Where(page =>
                        page.Site.Id == webpage.Site.Id && page.Id != webpage.Id && page.Name == webpage.Name)
                    .AnyAsync();

            if (anyWithSameTitle)
                return new List<SEOAnalysisFacet>
                {
                    GetFacet("Any other pages with same title?", SEOAnalysisStatus.Error, "The title is not unique")
                };
            return new List<SEOAnalysisFacet>
            {
                GetFacet("Any other pages with same title?", SEOAnalysisStatus.Success, "The title is unique")
            };
        }
    }
}