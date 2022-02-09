using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-65)]
    public class UpdateSiteDefaultLayouts : ICloneSiteParts
    {
        private readonly IConfigurationProviderFactory _factory;

        public UpdateSiteDefaultLayouts(IConfigurationProviderFactory factory)
        {
            _factory = factory;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var toProvider = _factory.GetForSite(to);
            var pageDefaultsSettings = toProvider.GetSiteSettings<PageDefaultsSettings>();

            var keys = pageDefaultsSettings.Layouts.Keys.ToList();
            foreach (var key in keys.Where(key => pageDefaultsSettings.Layouts[key].HasValue))
            {
                var layout = siteCloneContext.FindNew<Layout>(pageDefaultsSettings.Layouts[key].Value);
                if (layout != null)
                    pageDefaultsSettings.Layouts[key] = layout.Id;
            }

            await toProvider.SaveSettings(pageDefaultsSettings);
        }
    }
}