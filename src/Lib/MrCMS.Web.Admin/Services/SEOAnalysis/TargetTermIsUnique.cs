using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.SEOAnalysis;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Admin.Services.SEOAnalysis
{
    public class TargetTermIsUnique : BaseSEOAnalysisFacetProvider
    {
        private readonly ISession _session;

        public TargetTermIsUnique(ISession session)
        {
            _session = session;
        }

        public override async Task<IReadOnlyList<SEOAnalysisFacet>> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            var facets = new List<SEOAnalysisFacet>();
            var anyWithSameTitle =
                await _session.QueryOver<Webpage>()
                    .Where(page =>
                        page.Site.Id == webpage.Site.Id && page.Id != webpage.Id &&
                        page.SEOTargetPhrase.IsInsensitiveLike(webpage.SEOTargetPhrase, MatchMode.Exact))
                    .ListAsync();

            if (anyWithSameTitle.Any())
            {
                var messages = new List<string>
                {
                    "The target term is not unique, you should consider differentiating it from other pages within your site"
                };
                messages.AddRange(
                    anyWithSameTitle.Select(
                        page => $"<a href=\"/Admin/Webpage/Edit/{page.Id}\" target=\"_blank\">{page.Name}</a>"
                    ));
                facets.Add(
                    GetFacet("Any other pages with same target term?", SEOAnalysisStatus.Error, messages.ToArray()));
            }
            else
                facets.Add(
                    GetFacet("Any other pages with same target term?", SEOAnalysisStatus.Success,
                        "The target term is unique")
                );

            return facets;
        }
    }
}