using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class GetErrorPage : IGetErrorPage
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IConfigurationProvider _configurationProvider;

        public GetErrorPage(IRepository<Webpage> webpageRepository, IConfigurationProvider configurationProvider)
        {
            _webpageRepository = webpageRepository;
            _configurationProvider = configurationProvider;
        }

        public async Task<Webpage> GetPage(int code)
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            switch (code)
            {
                case 404:
                    return await _webpageRepository.Load(siteSettings.Error404PageId);
                case 401:
                case 403:
                    return await _webpageRepository.Load(siteSettings.Error403PageId);
                case 500:
                    return await _webpageRepository.Load(siteSettings.Error500PageId);
                default:
                    return null;
            }
        }
    }
}