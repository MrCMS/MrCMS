using System;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public abstract class DateTimeDataBase : BaseImmutableUserType<DateTime>
    {
        public override object NullSafeGet(DbDataReader dbDataReader, string[] names, ISessionImplementor session, object owner)
        {
            var nullSafeGet = NHibernateUtil.DateTime.NullSafeGet(dbDataReader, names[0], session, owner);
            var dateTime = DateTime.SpecifyKind((DateTime)nullSafeGet, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, GetTimeZone(session));
        }

        protected abstract TimeZoneInfo GetTimeZone(ISessionImplementor session);

        public override void NullSafeSet(DbCommand dbCommand, object value, int index, ISessionImplementor session)
        {
            //var dateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Unspecified);
            //// NOTE: This is a temporary work around to handle daylight savings correctly. 
            //var sourceTimeZone = GetTimeZone(session);
            //if (sourceTimeZone.IsInvalidTime(dateTime))
            //{
            //    var adjustmentRules = sourceTimeZone.GetAdjustmentRules();
            //    var adjustmentRule = adjustmentRules.FirstOrDefault();
            //    dateTime = dateTime.Add(adjustmentRule?.DaylightDelta ?? TimeSpan.FromHours(1));
            //}
            //dateTime = TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, TimeZoneInfo.Utc);
            var dateTime = (DateTime)value;
            NHibernateUtil.DateTime.NullSafeSet(dbCommand, dateTime.ToUniversalTime(), index, session);
        }

        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.DateTime.SqlType }; }
        }
    }
}