using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Events;
using MrCMS.Settings;

namespace MrCMS.Services.CloneSite
{
    [CloneSitePart(-100)]
    public class CopySettings : ICloneSiteParts
    {
        private readonly IGlobalRepository<Setting> _repository;

        public CopySettings(IGlobalRepository<Setting> repository)
        {
            _repository = repository;
        }

        public async Task Clone(Site @from, Site to, SiteCloneContext siteCloneContext)
        {
            var eventContext = new NullEventContext();
            var fromProvider = new SqlConfigurationProvider(_repository, @from, eventContext);// _legacySettingsProvider);
            var toProvider = new SqlConfigurationProvider(_repository, @to, eventContext);//, _legacySettingsProvider);
            var siteSettingsBases = fromProvider.GetAllSiteSettings();
            foreach (var settings in siteSettingsBases)
            {
                await toProvider.SaveSettings(settings);
            }
            //AppDataConfigurationProvider.ClearCache();
        }
    }
}