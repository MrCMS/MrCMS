using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-65)]
    public class UpdateSiteSettings : ICloneSiteParts
    {
        private readonly ILegacySettingsProvider _legacySettingsProvider;

        public UpdateSiteSettings(ILegacySettingsProvider legacySettingsProvider)
        {
            _legacySettingsProvider = legacySettingsProvider;
        }

        public void Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var fromSiteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var toSiteSettings = toProvider.GetSiteSettings<SiteSettings>();

            var error403 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error403PageId);
            if (error403 != null) toSiteSettings.Error403PageId = error403.Id;

            var error404 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error404PageId);
            if (error404 != null) toSiteSettings.Error404PageId = error404.Id;

            var error500 = siteCloneContext.FindNew<Webpage>(fromSiteSettings.Error500PageId);
            if (error500 != null) toSiteSettings.Error500PageId = error500.Id;

            var layout = siteCloneContext.FindNew<Layout>(fromSiteSettings.DefaultLayoutId);
            if (layout != null) toSiteSettings.DefaultLayoutId = layout.Id;

            toProvider.SaveSettings(toSiteSettings);
        }
    }
}