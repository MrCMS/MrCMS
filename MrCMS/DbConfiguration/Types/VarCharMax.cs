using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    public class VarcharMax : BaseImmutableUserType<String>
    {
        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            return (string)NHibernateUtil.String.NullSafeGet(rs, names[0]);
        }
        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            //Change the size of the parameter
            ((IDbDataParameter)cmd.Parameters[index]).Size = int.MaxValue;
            NHibernateUtil.String.NullSafeSet(cmd, value, index);
        }
        public override SqlType[] SqlTypes
        {
            get { return new[] { new SqlType(DbType.String) }; }
        }
    }
}