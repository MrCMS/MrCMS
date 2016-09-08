using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Routing
{
    public class GetErrorPage : IGetErrorPage
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly SiteSettings _siteSettings;

        public GetErrorPage(IRepository<Webpage> webpageRepository, SiteSettings siteSettings)
        {
            _webpageRepository = webpageRepository;
            _siteSettings = siteSettings;
        }

        public Webpage GetPage(int code)
        {
            switch (code)
            {
                case 404:
                    return _webpageRepository.Get(_siteSettings.Error404PageId);
                case 401:
                case 403:
                    return _webpageRepository.Get(_siteSettings.Error403PageId);
                case 500:
                    return _webpageRepository.Get(_siteSettings.Error500PageId);
                default:
                    return null;
            }
        }
    }
}