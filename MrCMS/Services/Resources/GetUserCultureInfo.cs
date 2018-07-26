using System.Globalization;
using MrCMS.Entities.People;
using MrCMS.Settings;

namespace MrCMS.Services.Resources
{
    public class GetUserCultureInfo : IGetUserCultureInfo
    {
        private readonly SiteSettings _siteSettings;

        public GetUserCultureInfo(SiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
        }

        public CultureInfo Get(User user)
        {
            var defaultCultureInfo = _siteSettings.CultureInfo;
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

        public string GetInfoString(User user)
        {
            return Get(user).Name;
        }
    }
}