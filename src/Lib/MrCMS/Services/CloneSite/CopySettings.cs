using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            var fromProvider = new SqlConfigurationProvider(_repository, SiteId.GetForSite(@from), eventContext);// _legacySettingsProvider);
            var toProvider = new SqlConfigurationProvider(_repository, SiteId.GetForSite(@to), eventContext);//, _legacySettingsProvider);
            var siteSettingsBases = await fromProvider.GetAllSiteSettings();
            foreach (var settings in siteSettingsBases)
            {
                await toProvider.SaveSettings(settings);
            }
            //AppDataConfigurationProvider.ClearCache();
        }
    }
}