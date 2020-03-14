using System;
using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class GetDateTimeNow : IGetDateTimeNow
    {
        private readonly IConfigurationProvider _configurationProvider;

        public GetDateTimeNow(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }


        public async Task<DateTime> GetLocalNow()
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, siteSettings.TimeZoneInfo);
        }

        public DateTime UtcNow => DateTime.UtcNow;
    }
}