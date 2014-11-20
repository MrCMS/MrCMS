using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-100)]
    public class CopySettings : ICloneSiteParts
    {
        private readonly ILegacySettingsProvider _legacySettingsProvider;

        public CopySettings(ILegacySettingsProvider legacySettingsProvider)
        {
            _legacySettingsProvider = legacySettingsProvider;
        }

        public void Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettingsBases = fromProvider.GetAllSiteSettings();
            siteSettingsBases.ForEach(toProvider.SaveSettings);
        }
    }
}