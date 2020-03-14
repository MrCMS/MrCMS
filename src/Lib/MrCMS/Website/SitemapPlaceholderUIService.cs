using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;


namespace MrCMS.Website
{
    public class SitemapPlaceholderUIService : ISitemapPlaceholderUIService
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IGetLiveUrl _getLiveUrl;

        public SitemapPlaceholderUIService(IRepository<Webpage> repository, IGetLiveUrl getLiveUrl)
        {
            _repository = repository;
            _getLiveUrl = getLiveUrl;
        }

        public async Task<RedirectResult> Redirect(SitemapPlaceholder page)
        {
            if (page == null)
                return new RedirectResult("~/");

            var child =
                _repository.Query().Where(webpage => webpage.ParentId == page.Id && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .FirstOrDefault();

            if (child == null)
                return new RedirectResult("~/");
            if (child.GetType().FullName != typeof(SitemapPlaceholder).FullName)
                return new RedirectResult($"~/{_getLiveUrl.GetUrlSegment(child)}");
            var lastRedirectChildUrl = await GetTheLastRedirectChildLink(child);
            return !string.IsNullOrWhiteSpace(lastRedirectChildUrl)
                ? new RedirectResult($"~/{lastRedirectChildUrl}")
                : new RedirectResult("~/");
        }

        private async Task<string> GetTheLastRedirectChildLink(Webpage sitemapPlaceholder)
        {
            var child =
                _repository.Query()
                    .Where(webpage => webpage.ParentId == sitemapPlaceholder.Id && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .FirstOrDefault();

            if (child == null)
                return string.Empty;

            if (child.DocumentClrType == typeof(SitemapPlaceholder).FullName)
                return await GetTheLastRedirectChildLink(child);

            return await _getLiveUrl.GetAbsoluteUrl(child);
        }
    }
}