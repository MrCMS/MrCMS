using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models.SEOAnalysis;


namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
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