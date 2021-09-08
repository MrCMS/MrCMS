using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Website
{
    public class SitemapPlaceholderUIService : ISitemapPlaceholderUIService
    {
        private readonly ISession _session;
        private readonly IGetLiveUrl _getLiveUrl;

        public SitemapPlaceholderUIService(ISession session, IGetLiveUrl getLiveUrl)
        {
            _session = session;
            _getLiveUrl = getLiveUrl;
        }

        public async Task<RedirectResult> Redirect(SitemapPlaceholder page)
        {
            if (page == null)
                return new RedirectResult("/", true);

            var child = await _session.QueryOver<Webpage>()
                .Where(webpage => webpage.Parent.Id == page.Id && webpage.Published)
                .OrderBy(webpage => webpage.DisplayOrder).Asc
                .Take(1).Cacheable().SingleOrDefaultAsync();

            if (child == null)
                return new RedirectResult("/", true);
            if (child.GetType().FullName != typeof(SitemapPlaceholder).FullName)
                return new RedirectResult(await _getLiveUrl.GetAbsoluteUrl(child), true);
            var lastRedirectChildUrl = await GetTheLastRedirectChildLink(child);
            return !string.IsNullOrWhiteSpace(lastRedirectChildUrl)
                ? new RedirectResult(lastRedirectChildUrl, true)
                : new RedirectResult("/", true);
        }

        private async Task<string> GetTheLastRedirectChildLink(Webpage sitemapPlaceholder)
        {
            var child = await
                _session.QueryOver<Webpage>()
                    .Where(webpage => webpage.Parent.Id == sitemapPlaceholder.Id && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder).Asc
                    .Take(1).Cacheable().SingleOrDefaultAsync();

            if (child == null)
                return string.Empty;

            if (child.DocumentType == typeof(SitemapPlaceholder).FullName)
                return await GetTheLastRedirectChildLink(child);

            return await _getLiveUrl.GetAbsoluteUrl(child);
        }
    }
}