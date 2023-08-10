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

        public CultureInfo Get(string userUiCulture)
        {
            var defaultCultureInfo = _siteSettings.CultureInfo;
            if (string.IsNullOrWhiteSpace(userUiCulture))
                return defaultCultureInfo;
            try
            {
                return CultureInfo.GetCultureInfo(userUiCulture);
            }
            catch
            {
                return defaultCultureInfo;
            }
        }

        public string GetInfoString(string userUiCulture)
        {
            return Get(userUiCulture).Name;
        }
    }
}