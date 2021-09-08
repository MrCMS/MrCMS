using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-65)]
    public class UpdateSiteSettings : ICloneSiteParts
    {
        private readonly IConfigurationProviderFactory _factory;

        public UpdateSiteSettings(IConfigurationProviderFactory factory)
        {
            _factory = factory;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var fromProvider = _factory.GetForSite(@from);
            var fromSiteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var toProvider = _factory.GetForSite(to);
            var toSiteSettings = toProvider.GetSiteSettings<SiteSettings>();

            var homepage = siteCloneContext.FindNew<Webpage>(fromSiteSettings.HomePageId);
            if (homepage != null) toSiteSettings.HomePageId = homepage.Id;

            var layout = siteCloneContext.FindNew<Layout>(fromSiteSettings.DefaultLayoutId);
            if (layout != null) toSiteSettings.DefaultLayoutId = layout.Id;

            await toProvider.SaveSettings(toSiteSettings);
        }
    }
}