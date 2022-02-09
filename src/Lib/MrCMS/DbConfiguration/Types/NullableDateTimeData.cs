using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate.Engine;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class NullableDateTimeData : NullableDateTimeDataBase
    {
        protected override TimeZoneInfo GetTimeZone(ISessionImplementor session)
        {
            var requiredService = session.GetContext()?.RequestServices?.GetRequiredService<IConfiguration>();
            var s = new SystemConfig();
            requiredService?.GetSection(SystemConfig.SectionName)?.Bind(s);
            return s?.TimeZoneInfo ?? TimeZoneInfo.Local;
        }
    }
}