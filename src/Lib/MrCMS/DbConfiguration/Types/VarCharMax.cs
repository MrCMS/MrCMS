using System;
using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class VarcharMax : BaseImmutableUserType<String>
    {
        public override object NullSafeGet(DbDataReader dbDataReader, string[] names, ISessionImplementor session, object owner)
        {
            return (string)NHibernateUtil.String.NullSafeGet(dbDataReader, names[0], session);
        }
        public override void NullSafeSet(DbCommand dbCommand, object value, int index, ISessionImplementor session)
        {
            //Change the size of the parameter
            ((IDbDataParameter)dbCommand.Parameters[index]).Size = int.MaxValue;
            NHibernateUtil.String.NullSafeSet(dbCommand, value, index, session);
        }
        public override SqlType[] SqlTypes
        {
            get { return new[] { new SqlType(DbType.String) }; }
        }
    }
}