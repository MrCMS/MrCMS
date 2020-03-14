using System.Globalization;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Settings;

namespace MrCMS.Services.Resources
{
    public class GetUserCultureInfo : IGetUserCultureInfo
    {
        private readonly IConfigurationProvider _configurationProvider;

        public GetUserCultureInfo(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task<CultureInfo> Get(User user)
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            var defaultCultureInfo = siteSettings.CultureInfo;
            if (user == null || string.IsNullOrWhiteSpace(user.UICulture))
                return defaultCultureInfo;
            try
            {
                return CultureInfo.GetCultureInfo(user.UICulture);
            }
            catch
            {
                return defaultCultureInfo;
            }
        }

        public async Task<string> GetInfoString(User user)
        {
            return (await Get(user)).Name;
        }
    }
}