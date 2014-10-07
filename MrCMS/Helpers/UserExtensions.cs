using System.Globalization;
using MrCMS.Entities.People;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class UserExtensions
    {
        public static CultureInfo GetUICulture(this User user, CultureInfo defaultCultureInfo = null)
        {
            defaultCultureInfo = defaultCultureInfo ?? MrCMSApplication.Get<SiteSettings>().CultureInfo;
            if (string.IsNullOrWhiteSpace(user.UICulture))
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
    }
}