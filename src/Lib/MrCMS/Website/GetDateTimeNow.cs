using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public class GetDateTimeNow : IGetDateTimeNow
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IOptions<SystemConfigurationSettings> _options;

        public GetDateTimeNow(IConfigurationProvider configurationProvider, IOptions<SystemConfigurationSettings> options)
        {
            _configurationProvider = configurationProvider;
            _options = options;
        }
        
        public async Task<DateTime> GetLocalNow()
        {
            return await Task.FromResult(TimeZoneInfo.ConvertTime(DateTime.UtcNow, _options.Value.TimeZoneInfo));
        }

        public DateTime UtcNow => DateTime.UtcNow;

    }
}