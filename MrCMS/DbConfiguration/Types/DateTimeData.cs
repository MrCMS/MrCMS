using System;
using NHibernate.Engine;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class DateTimeData : DateTimeDataBase
    {
        protected override TimeZoneInfo GetTimeZone(ISessionImplementor session)
        {
            return TimeZoneInfo.Local;
        }
    }
}