using System;
using MrCMS.Website;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class NullableDateTimeData : NullableDateTimeDataBase
    {
        protected override TimeZoneInfo TimeZone
        {
            get { return CurrentRequestData.TimeZoneInfo; }
        }
    }
}