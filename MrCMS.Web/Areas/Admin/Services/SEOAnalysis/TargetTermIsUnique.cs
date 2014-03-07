using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using HtmlAgilityPack;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class TargetTermIsUnique : BaseSEOAnalysisFacetProvider
    {
        private readonly ISession _session;

        public TargetTermIsUnique(ISession session)
        {
            _session = session;
        }

        public override IEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var anyWithSameTitle =
                _session.QueryOver<Webpage>()
                    .Where(page => page.Site.Id == webpage.Site.Id && page.Id != webpage.Id && page.SEOTargetPhrase.IsInsensitiveLike(webpage.SEOTargetPhrase, MatchMode.Exact))
                    .List();

            if (anyWithSameTitle.Any())
            {
                var messages = new List<string> { "The target term is not unique, you should consider differentiating it from other pages within your site" };
                messages.AddRange(
                    anyWithSameTitle.Select(
                        page => string.Format("<a href=\"/Admin/Webpage/Edit/{0}\" target=\"_blank\">{1}</a>", page.Id, page.Name)
                            ));
                yield return
                    GetFacet("Any other pages with same target term?", SEOAnalysisStatus.Error, messages.ToArray());
            }
            else
                yield return
                    GetFacet("Any other pages with same target term?", SEOAnalysisStatus.Success, "The target term is unique");
        }
    }
}