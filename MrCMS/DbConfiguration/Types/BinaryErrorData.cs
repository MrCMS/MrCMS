using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    public class BinaryData<T> : BaseImmutableUserType<T>
    {
        readonly BinaryFormatter binaryFormatter = new BinaryFormatter();
        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var o = NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0]) as byte[];
            using (var memoryStream = new MemoryStream(o))
            {
                try
                {
                    var deserialize = binaryFormatter.Deserialize(memoryStream);
                    return deserialize;
                }
                catch
                {
                    return null;
                }

            }
        }

        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            ((IDbDataParameter)cmd.Parameters[index]).Size = int.MaxValue;

            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, value);
                NHibernateUtil.BinaryBlob.NullSafeSet(cmd, memoryStream.GetBuffer(), index);
            }
        }

        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.BinaryBlob.SqlType }; }
        }
    }
}