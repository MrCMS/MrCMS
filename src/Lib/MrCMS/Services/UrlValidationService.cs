using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Services
{
    public class UrlValidationService : IUrlValidationService
    {
        private readonly ISession _session;

        public UrlValidationService(ISession session)
        {
            _session = session;
        }

        public bool UrlIsValidForWebpage(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = _session.Get<Webpage>(id.Value);
                var documentHistory = document.Urls.Any(x => x.UrlSegment == url);
                if (url.Trim() == document.UrlSegment.Trim() || documentHistory) //if url is the same or has been used for the same page before lets go
                    return true;

                return !WebpageExists(url) && !ExistsInUrlHistory(url);
            }

            return !WebpageExists(url) && !ExistsInUrlHistory(url);
        }

        /// <summary>
        /// Check to see if the supplied URL is ok to be added to the URL history table
        /// </summary>
        public bool UrlIsValidForWebpageUrlHistory(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return !WebpageExists(url) && !ExistsInUrlHistory(url);
        }

        public bool UrlIsValidForMediaCategory(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = _session.Get<MediaCategory>(id.Value);
                if (url.Trim() == document.UrlSegment.Trim())
                    return true;
                return !MediaCategoryExists(url);
            }

            return !MediaCategoryExists(url);
        }

        public bool UrlIsValidForLayout(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = _session.Get<Layout>(id.Value);
                if (url.Trim() == document.UrlSegment.Trim())
                    return true;
                return !LayoutExists(url);
            }

            return !LayoutExists(url);
        }



        /// <summary>
        /// Checks to see if a webpage exists with the supplied URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private bool WebpageExists(string url)
        {
            return _session.QueryOver<Webpage>()
                .Where(doc => doc.UrlSegment == url)
                .Cacheable()
                .RowCount() > 0;
        }

        /// <summary>
        /// Checks to see if the supplied URL exists in webpage URL history table
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private bool ExistsInUrlHistory(string url)
        {
            return
                _session.QueryOver<UrlHistory>()
                    .Where(doc => doc.UrlSegment == url)
                    .Cacheable()
                    .RowCount() > 0;
        }

        /// <summary>
        /// Checks to see if the supplied url is unique for media category / folder.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private bool MediaCategoryExists(string url)
        {
            return _session.QueryOver<MediaCategory>()
                .Where(doc => doc.UrlSegment == url)
                .Cacheable()
                .RowCount() > 0;
        }

        /// <summary>
        /// Checks to see if the supplied url is unique for layouts
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool LayoutExists(string url)
        {
            return _session.QueryOver<Layout>()
                .Where(doc => doc.UrlSegment == url)
                .Cacheable()
                .RowCount() > 0;
        }
    }
}