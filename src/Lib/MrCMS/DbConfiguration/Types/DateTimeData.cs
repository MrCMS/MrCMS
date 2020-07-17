using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate.Engine;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class DateTimeData : DateTimeDataBase
    {
        protected override TimeZoneInfo GetTimeZone(ISessionImplementor session)
        {
            var requiredService = session.GetContext()?.RequestServices?.GetRequiredService<IConfiguration>();
            var s = new SystemConfig();
            requiredService?.GetSection("SystemConfig")?.Bind(s);
            return s?.TimeZoneInfo ?? TimeZoneInfo.Local;
        }
    }
}