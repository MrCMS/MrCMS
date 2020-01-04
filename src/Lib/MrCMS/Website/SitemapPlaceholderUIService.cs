using System.Linq;
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

        public RedirectResult Redirect(SitemapPlaceholder page)
        {
            if (page == null)
                return new RedirectResult("~");

            var child =
                _repository.Query().Where(webpage => webpage.ParentId == page.Id && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .FirstOrDefault();

            if (child == null)
                return new RedirectResult("~");
            if (child.GetType().FullName != typeof(SitemapPlaceholder).FullName)
                return new RedirectResult($"~/{_getLiveUrl.GetUrlSegment(child)}");
            var lastRedirectChildUrl = GetTheLastRedirectChildLink(child);
            return !string.IsNullOrWhiteSpace(lastRedirectChildUrl)
                ? new RedirectResult($"~/{lastRedirectChildUrl}")
                : new RedirectResult("~");
        }

        private string GetTheLastRedirectChildLink(Webpage sitemapPlaceholder)
        {
            var child =
                _repository.Query()
                    .Where(webpage => webpage.ParentId == sitemapPlaceholder.Id && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .FirstOrDefault();

            if (child == null)
                return string.Empty;

            if (child.DocumentType == typeof(SitemapPlaceholder).FullName)
                GetTheLastRedirectChildLink(child);

            return _getLiveUrl.GetAbsoluteUrl(child);
        }
    }
}