using System;
using System.Data;
using System.Linq;
using MrCMS.Website;
using NHibernate;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class DateTimeData : DateTimeDataBase
    {
        protected override TimeZoneInfo TimeZone { get { return CurrentRequestData.TimeZoneInfo; } }
    }
}