using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class UrlValidationService : IUrlValidationService
    {
        private readonly IDataReader _dataReader;

        public UrlValidationService(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }

        public async Task<bool> UrlIsValidForWebpage(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = await _dataReader.Get<Webpage>(id.Value);
                var documentHistory = document.Urls.Any(x => x.UrlSegment == url);
                if (url.Trim() == document.UrlSegment.Trim() || documentHistory) //if url is the same or has been used for the same page before lets go
                    return true;

                return !await WebpageExists(url) && !await ExistsInUrlHistory(url);
            }

            return !await WebpageExists(url) && !await ExistsInUrlHistory(url);
        }

        /// <summary>
        /// Check to see if the supplied URL is ok to be added to the URL history table
        /// </summary>
        public async Task<bool> UrlIsValidForWebpageUrlHistory(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return !await WebpageExists(url) && !await ExistsInUrlHistory(url);
        }

        public async Task<bool> UrlIsValidForMediaCategory(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = await _dataReader.Get<MediaCategory>(id.Value);
                if (url.Trim() == document.UrlSegment.Trim())
                    return true;
                return !await MediaCategoryExists(url);
            }

            return !await MediaCategoryExists(url);
        }

        public async Task<bool> UrlIsValidForLayout(string url, int? id)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (id.HasValue)
            {
                var document = await _dataReader.Get<Layout>(id.Value);
                if (url.Trim() == document.UrlSegment.Trim())
                    return true;
                return !await LayoutExists(url);
            }

            return !await LayoutExists(url);
        }



        /// <summary>
        /// Checks to see if a webpage exists with the supplied URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private Task<bool> WebpageExists(string url)
        {
            return _dataReader.Readonly<Webpage>()
                .AnyAsync(doc => doc.UrlSegment == url);

        }

        /// <summary>
        /// Checks to see if the supplied URL exists in webpage URL history table
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private Task<bool> ExistsInUrlHistory(string url)
        {
            return
                _dataReader.Readonly<UrlHistory>()
                    .AnyAsync(doc => doc.UrlSegment == url);
        }

        /// <summary>
        /// Checks to see if the supplied url is unique for media category / folder.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>bool</returns>
        private Task<bool> MediaCategoryExists(string url)
        {
            return _dataReader.Readonly<MediaCategory>()
                    .AnyAsync(doc => doc.UrlSegment == url) ;
        }

        /// <summary>
        /// Checks to see if the supplied url is unique for layouts
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Task<bool> LayoutExists(string url)
        {
            return _dataReader.Readonly<Layout>()
                    .AnyAsync(doc => doc.UrlSegment == url)
                ;
        }
    }
}