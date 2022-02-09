using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-100)]
    public class CopySettings : ICloneSiteParts
    {
        private readonly IConfigurationProviderFactory _factory;

        public CopySettings(IConfigurationProviderFactory factory)
        {
            _factory = factory;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var fromProvider = _factory.GetForSite(@from);
            var toProvider = _factory.GetForSite(to);
            var siteSettingsBases = fromProvider.GetAllSiteSettings();
            foreach (var @base in siteSettingsBases)
            {
                await toProvider.SaveSettings(@base);
            }
        }
    }
}