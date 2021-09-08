using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class GetWebpageForPath : IGetWebpageForPath
    {
        private readonly IGetDocumentByUrl<Webpage> _getWebpageByUrl;
        private readonly IGetHomePage _getHomePage;

        public GetWebpageForPath(IGetDocumentByUrl<Webpage> getWebpageByUrl, IGetHomePage getHomePage)
        {
            _getWebpageByUrl = getWebpageByUrl;
            _getHomePage = getHomePage;
        }

        public async Task<Webpage> GetWebpage(string path)
        {
            var url = path?.TrimStart('/');
            return
                string.IsNullOrWhiteSpace(url)
                    ? await _getHomePage.Get()
                    : await _getWebpageByUrl.GetByUrl(url);
        }
    }
}