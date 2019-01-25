using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public abstract class NullableDateTimeDataBase : BaseImmutableUserType<DateTime?>
    {
        public override object NullSafeGet(DbDataReader dbDataReader, string[] names, ISessionImplementor session, object owner)
        {
            var nullSafeGet = NHibernateUtil.UtcDateTime.NullSafeGet(dbDataReader, names[0], session, owner);
            if (nullSafeGet == null)
                return null;
            var dateTime = DateTime.SpecifyKind((DateTime)nullSafeGet, DateTimeKind.Unspecified);
            var timeZone = GetTimeZone(session);
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, timeZone);
        }

        protected abstract TimeZoneInfo GetTimeZone(ISessionImplementor session);

        public override void NullSafeSet(DbCommand dbCommand, object value, int index, ISessionImplementor session)
        {
            if (value != null)
            {
                var dateTime = (DateTime)value;
                if (dateTime.Kind != DateTimeKind.Utc)
                {
                    var zoneInfo = GetTimeZone(session);
                // NOTE: This is a work around to handle daylight savings correctly. 
                var sourceTimeZone = GetTimeZone(session);
                if (sourceTimeZone.IsInvalidTime(dateTime))
                {
                    var adjustmentRules = sourceTimeZone.GetAdjustmentRules();
                    var adjustmentRule = adjustmentRules.FirstOrDefault();
                    dateTime = dateTime.Add(adjustmentRule?.DaylightDelta ?? TimeSpan.FromHours(1));
                }
                    dateTime = TimeZoneInfo.ConvertTime(dateTime, zoneInfo, TimeZoneInfo.Utc);
                }

                NHibernateUtil.UtcDateTime.NullSafeSet(dbCommand, dateTime.ToUniversalTime(), index, session);
            }
            else
                NHibernateUtil.DateTime.NullSafeSet(dbCommand, null, index, session);
        }

        public override SqlType[] SqlTypes => new[] { NHibernateUtil.DateTime.SqlType };
    }
}