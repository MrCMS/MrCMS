using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Settings;

namespace MrCMS.Services.Sitemaps
{
    public class ExcludeFromSitemapBecauseIsErrorPage : IReasonToExcludePageFromSitemap
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ExcludeFromSitemapBecauseIsErrorPage(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task<bool> ShouldExclude(Webpage webpage)
        {
            if (webpage == null)
                return true;
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            return siteSettings.Error403PageId == webpage.Id
                   || siteSettings.Error404PageId == webpage.Id
                   || siteSettings.Error500PageId == webpage.Id;
        }
    }
}