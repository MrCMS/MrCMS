using System;
using System.Data;
using MrCMS.Website;
using NHibernate;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    public class DateTimeData : BaseImmutableUserType<DateTime>
    {
        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var dateTime = (DateTime)NHibernateUtil.DateTime.NullSafeGet(rs, names[0]);
            return dateTime.Add(CurrentRequestData.TimeZoneInfo.BaseUtcOffset);
        }

        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var dateTime = (DateTime)value;
            dateTime = dateTime.Subtract(CurrentRequestData.TimeZoneInfo.BaseUtcOffset);
            NHibernateUtil.DateTime.NullSafeSet(cmd, dateTime, index);
        }

        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.DateTime.SqlType }; }
        }
    }
    public class NullableDateTimeData : BaseImmutableUserType<DateTime?>
    {
        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var nullSafeGet = NHibernateUtil.DateTime.NullSafeGet(rs, names[0]);
            if (nullSafeGet == null)
                return null;
            var dateTime = (DateTime)nullSafeGet;
            return dateTime.Add(CurrentRequestData.TimeZoneInfo.BaseUtcOffset);
        }

        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value != null)
            {
                var dateTime = (DateTime) value;
                dateTime = dateTime.Subtract(CurrentRequestData.TimeZoneInfo.BaseUtcOffset);
                NHibernateUtil.DateTime.NullSafeSet(cmd, dateTime, index);
            }
            else
            {
                NHibernateUtil.DateTime.NullSafeSet(cmd, value, index);
            }
        }

        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.DateTime.SqlType }; }
        }
    }
}