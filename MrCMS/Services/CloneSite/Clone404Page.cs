using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-70)]
    public class Clone404Page : ICloneSiteParts
    {
        private readonly ISession _session;
        private readonly ILegacySettingsProvider _legacySettingsProvider;

        public Clone404Page(ISession session, ILegacySettingsProvider legacySettingsProvider)
        {
            _session = session;
            _legacySettingsProvider = legacySettingsProvider;
        }

        public void Clone(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(@from, _legacySettingsProvider);
            var toProvider = new ConfigurationProvider(@to, _legacySettingsProvider);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error404 = _session.Get<Webpage>(siteSettings.Error404PageId);

            var copy = error404.GetCopyForSite(to);
            _session.Transact(session => session.Save(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error404PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }
    }
}