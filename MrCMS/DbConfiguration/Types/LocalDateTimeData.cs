using System;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class LocalDateTimeData : BaseImmutableUserType<DateTime>
    {
        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var dateTime = DateTime.SpecifyKind((DateTime)NHibernateUtil.DateTime.NullSafeGet(rs, names[0]),
                                                DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, TimeZoneInfo.Local);
        }

        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var dateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Unspecified);
            // NOTE: This is a temporary work around to handle daylight savings correctly. 
            var sourceTimeZone = TimeZoneInfo.Local;
            if (sourceTimeZone.IsInvalidTime(dateTime))
            {
                var adjustmentRules = sourceTimeZone.GetAdjustmentRules();
                var adjustmentRule = adjustmentRules.FirstOrDefault();
                dateTime = dateTime.Add(adjustmentRule?.DaylightDelta ?? TimeSpan.FromHours(1));
            }
            dateTime = TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, TimeZoneInfo.Utc);
            NHibernateUtil.DateTime.NullSafeSet(cmd, dateTime, index);
        }

        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.DateTime.SqlType }; }
        }
    }
}