using System;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class LocalDateTimeData : DateTimeDataBase
    {
        protected override TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.Local; }
        }
    }
}