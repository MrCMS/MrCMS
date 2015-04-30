using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Website.Controllers
{
    public class SitemapPlaceholderController : MrCMSUIController
    {
        private readonly ISitemapPlaceholderUIService _sitemapPlaceholderUIService;

        public SitemapPlaceholderController(ISitemapPlaceholderUIService sitemapPlaceholderUIService)
        {
            _sitemapPlaceholderUIService = sitemapPlaceholderUIService;
        }

        public RedirectResult Show(SitemapPlaceholder page)
        {
            return _sitemapPlaceholderUIService.Redirect(page);
        }
    }

    public interface ISitemapPlaceholderUIService
    {
        RedirectResult Redirect(SitemapPlaceholder page);
    }

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
            return child == null
                ? new RedirectResult("~")
                : new RedirectResult(string.Format("~/{0}", child.LiveUrlSegment));
        }
    }
}