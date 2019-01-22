using System;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class GetDateTimeNow : IGetDateTimeNow
    {
        private readonly SiteSettings _siteSettings;

        public GetDateTimeNow(SiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
        }

        public DateTime LocalNow => TimeZoneInfo.ConvertTime(DateTime.UtcNow, _siteSettings.TimeZoneInfo);
        public DateTime UtcNow => DateTime.UtcNow;
    }
}