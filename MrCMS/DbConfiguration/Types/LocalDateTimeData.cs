using System;
using System.Data;
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
            dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, TimeZoneInfo.Utc);
            NHibernateUtil.DateTime.NullSafeSet(cmd, dateTime, index);
        }

        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.DateTime.SqlType }; }
        }
    }
}