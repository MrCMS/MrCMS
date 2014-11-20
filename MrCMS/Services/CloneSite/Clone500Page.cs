using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-60)]
    public class Clone500Page : ICloneSiteParts
    {
        private readonly ISession _session;
        private readonly ILegacySettingsProvider _legacySettingsProvider;

        public Clone500Page(ISession session, ILegacySettingsProvider legacySettingsProvider)
        {
            _session = session;
            _legacySettingsProvider = legacySettingsProvider;
        }
        public void Clone(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error500 = _session.Get<Webpage>(siteSettings.Error500PageId);

            var copy = error500.GetCopyForSite(to);
            _session.Transact(session => session.Save(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error500PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }
    }
}