using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-65)]
    public class Clone403Page : ICloneSiteParts
    {
        private readonly ISession _session;
        private readonly ILegacySettingsProvider _legacySettingsProvider;

        public Clone403Page(ISession session, ILegacySettingsProvider legacySettingsProvider)
        {
            _session = session;
            _legacySettingsProvider = legacySettingsProvider;
        }
        public void Clone(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error403 = _session.Get<Webpage>(siteSettings.Error403PageId);

            var copy = error403.GetCopyForSite(to);
            _session.Transact(session => session.Save(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error403PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }
    }
}