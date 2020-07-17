using System;
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
            return session.GetContext().Request.HttpContext.RequestServices.GetRequiredService<IOptions<SystemConfig>>()?.Value.TimeZoneInfo ?? TimeZoneInfo.Local;
        }
    }
}