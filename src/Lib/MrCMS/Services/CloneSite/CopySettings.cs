using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-100)]
    public class CopySettings : ICloneSiteParts
    {
        private readonly ISession _session;

        public CopySettings(ISession session)
        {
            _session = session;
        }

        public void Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var fromProvider = new SqlConfigurationProvider(_session, @from);// _legacySettingsProvider);
            var toProvider = new SqlConfigurationProvider(_session, @to);//, _legacySettingsProvider);
            var siteSettingsBases = fromProvider.GetAllSiteSettings();
            siteSettingsBases.ForEach(toProvider.SaveSettings);
            //AppDataConfigurationProvider.ClearCache();
        }
    }
}