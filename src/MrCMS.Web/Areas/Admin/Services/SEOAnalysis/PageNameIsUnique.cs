using System.Collections.Generic;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models.SEOAnalysis;

namespace MrCMS.Web.Areas.Admin.Services.SEOAnalysis
{
    public class PageNameIsUnique : BaseSEOAnalysisFacetProvider
    {
        private readonly IRepository<Webpage> _repository;

        public PageNameIsUnique(IRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public override async IAsyncEnumerable<SEOAnalysisFacet> GetFacets(Webpage webpage, HtmlNode document,
            string analysisTerm)
        {
            bool anyWithSameTitle = await
                _repository
                    .Query()
                    .AnyAsync(page => page.Id != webpage.Id && page.Name == webpage.Name);

            if (anyWithSameTitle)
                yield return
                    await GetFacet("Any other pages with same title?", SEOAnalysisStatus.Error, "The title is not unique");
            else
                yield return
                    await GetFacet("Any other pages with same title?", SEOAnalysisStatus.Success, "The title is unique");
        }
    }
}