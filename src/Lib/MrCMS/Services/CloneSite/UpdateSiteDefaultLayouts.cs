using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Events;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-65)]
    public class UpdateSiteDefaultLayouts : ICloneSiteParts
    {
        private readonly IGlobalRepository<Setting> _repository;

        public UpdateSiteDefaultLayouts(IGlobalRepository<Setting> repository)
        {
            _repository = repository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var toProvider = new SqlConfigurationProvider(_repository, SiteId.GetForSite(@to), new NullEventContext());
            var pageDefaultsSettings = await toProvider.GetSiteSettings<PageDefaultsSettings>();

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