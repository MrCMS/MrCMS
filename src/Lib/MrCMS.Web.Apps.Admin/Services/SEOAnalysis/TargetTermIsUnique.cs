using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;


namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class TargetTermIsUnique : BaseSEOAnalysisFacetProvider
    {
        private readonly IRepository<Webpage> _repository;

        public TargetTermIsUnique(IRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public override async IAsyncEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document, string analysisTerm)
        {
            var anyWithSameTitle =
                await _repository.Readonly()
                    .Where(page => page.Site.Id == webpage.Site.Id && page.Id != webpage.Id && EF.Functions.Like(page.SEOTargetPhrase, analysisTerm))
                    .ToListAsync();

            if (anyWithSameTitle.Any())
            {
                var messages = new List<string> { "The target term is not unique, you should consider differentiating it from other pages within your site" };
                messages.AddRange(
                    anyWithSameTitle.Select(
                        page => string.Format("<a href=\"/Admin/Webpage/Edit/{0}\" target=\"_blank\">{1}</a>", page.Id, page.Name)
                            ));
                yield return
                    await GetFacet("Any other pages with same target term?", SEOAnalysisStatus.Error, messages.ToArray());
            }
            else
                yield return
                    await GetFacet("Any other pages with same target term?", SEOAnalysisStatus.Success, "The target term is unique");
        }
    }
}