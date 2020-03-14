using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Events;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-65)]
    public class UpdateSiteSettings : ICloneSiteParts
    {
        private readonly IGlobalRepository<Setting> _repository;

        public UpdateSiteSettings(IGlobalRepository<Setting> repository)
        {
            _repository = repository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var nullEventContext = new NullEventContext();
            var fromProvider = new SqlConfigurationProvider(_repository, SiteId.GetForSite(@from), nullEventContext);
            var fromSiteSettings = await fromProvider.GetSiteSettings<SiteSettings>();
            var toProvider = new SqlConfigurationProvider(_repository, SiteId.GetForSite(@to), nullEventContext);
            var toSiteSettings = await toProvider.GetSiteSettings<SiteSettings>();

            var error403 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error403PageId);
            if (error403 != null) toSiteSettings.Error403PageId = error403.Id;

            var error404 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error404PageId);
            if (error404 != null) toSiteSettings.Error404PageId = error404.Id;

            var error500 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error500PageId);
            if (error500 != null) toSiteSettings.Error500PageId = error500.Id;

            var layout = siteCloneContext.FindNew<Layout>(fromSiteSettings.DefaultLayoutId);
            if (layout != null) toSiteSettings.DefaultLayoutId = layout.Id;

            await toProvider.SaveSettings(toSiteSettings);
        }
    }
}