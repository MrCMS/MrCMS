using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate.Engine;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class DateTimeData : DateTimeDataBase
    {
        protected override TimeZoneInfo GetTimeZone(ISessionImplementor session)
        {
            var requiredService = session.GetContext()?.RequestServices?.GetRequiredService<IConfiguration>();
            if (requiredService == null) return TimeZoneInfo.Local;
            var s = new SystemConfig();
            requiredService?.GetSection("SystemConfig")?.Bind(s);
            return s?.TimeZoneInfo ?? TimeZoneInfo.Local;
        }
    }
}