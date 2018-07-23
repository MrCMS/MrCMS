using System;
using MrCMS.Website;
using NHibernate.Engine;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class NullableDateTimeData : NullableDateTimeDataBase
    {
        protected override TimeZoneInfo GetTimeZone(ISessionImplementor session)
        {
            // TODO: timezone
            return TimeZoneInfo.Local;
            //return CurrentRequestData.TimeZoneInfo;
        }
    }
}