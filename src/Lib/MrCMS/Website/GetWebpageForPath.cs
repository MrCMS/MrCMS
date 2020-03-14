using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class GetWebpageForPath : IGetWebpageForPath
    {
        private readonly IGetDocumentByUrl<Webpage> _getWebpageByUrl;
        private readonly IGetHomePage _getHomePage;
        private readonly ICacheInHttpContext _cacheInHttpContext;

        public GetWebpageForPath(IGetDocumentByUrl<Webpage> getWebpageByUrl, IGetHomePage getHomePage, ICacheInHttpContext cacheInHttpContext)
        {
            _getWebpageByUrl = getWebpageByUrl;
            _getHomePage = getHomePage;
            _cacheInHttpContext = cacheInHttpContext;
        }

        public async Task<Webpage> GetWebpage(string path)
        {
            var url = path?.TrimStart('/');
            //return await _cacheInHttpContext.GetForRequest($"webpage-for-path|{url}", () => string.IsNullOrWhiteSpace(url)
            //        ? _getHomePage.Get()
            //        : _getWebpageByUrl.GetByUrl(url));
            return string.IsNullOrWhiteSpace(url)
                ? await _getHomePage.Get()
                : await _getWebpageByUrl.GetByUrl(url);
        }
    }
}