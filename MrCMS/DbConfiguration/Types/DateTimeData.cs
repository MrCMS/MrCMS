using System;
using MrCMS.Website;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class DateTimeData : DateTimeDataBase
    {
        protected override TimeZoneInfo TimeZone
        {
            get { return CurrentRequestData.TimeZoneInfo; }
        }
    }
}