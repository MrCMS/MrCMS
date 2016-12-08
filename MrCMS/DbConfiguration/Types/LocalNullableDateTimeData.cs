using System;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class LocalNullableDateTimeData : NullableDateTimeDataBase
    {
        protected override TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.Local; }
        }
    }
}