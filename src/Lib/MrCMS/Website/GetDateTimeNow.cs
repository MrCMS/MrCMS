using System;
using Microsoft.Extensions.Options;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class GetDateTimeNow : IGetDateTimeNow
    {
        private readonly IOptions<SystemConfig> _config;

        public GetDateTimeNow(IOptions<SystemConfig> config)
        {
            _config = config;
        }

        public DateTime LocalNow => TimeZoneInfo.ConvertTime(DateTime.UtcNow, _config.Value.TimeZoneInfo);
        public DateTime UtcNow => DateTime.UtcNow;
    }
}