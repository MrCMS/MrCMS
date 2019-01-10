using System;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class GetNowForSite : IGetNowForSite
    {
        private readonly SiteSettings _siteSettings;

        public GetNowForSite(SiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
        }

        public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, _siteSettings.TimeZoneInfo);
    }
}