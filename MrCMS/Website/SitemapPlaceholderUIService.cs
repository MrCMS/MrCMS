using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Website
{
    public class SitemapPlaceholderUIService : ISitemapPlaceholderUIService
    {
        private readonly ISession _session;

        public SitemapPlaceholderUIService(ISession session)
        {
            _session = session;
        }

        public RedirectResult Redirect(SitemapPlaceholder page)
        {
            if (page == null)
                return new RedirectResult("~");

            var child =
                _session.QueryOver<Webpage>().Where(webpage => webpage.Parent.Id == page.Id && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder).Asc
                    .Take(1).Cacheable().List().FirstOrDefault();

            if (child == null)
                return new RedirectResult("~");
            if (child.GetType().FullName != typeof(SitemapPlaceholder).FullName)
                return new RedirectResult($"~/{child.LiveUrlSegment}");
            var lastRedirectChildUrl = GetTheLastRedirectChildLink(child);
            return !string.IsNullOrWhiteSpace(lastRedirectChildUrl) ? new RedirectResult($"~/{lastRedirectChildUrl}") : new RedirectResult("~");
        }

        private string GetTheLastRedirectChildLink(Webpage sitemapPlaceholder)
        {
            var child =
                _session.QueryOver<Webpage>().Where(webpage => webpage.Parent.Id == sitemapPlaceholder.Id && webpage.Published)
                    .OrderBy(webpage => webpage.DisplayOrder).Asc
                    .Take(1).Cacheable().List().FirstOrDefault();

            if (child == null)
                return string.Empty;

            if (child.DocumentType == typeof(SitemapPlaceholder).FullName)
                GetTheLastRedirectChildLink(child);

            return child.LiveUrlSegment;
        }
    }
}