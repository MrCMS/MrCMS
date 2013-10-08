using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    public class BinaryData<T> : BaseImmutableUserType<T>
    {
        public override object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var o = NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0]) as byte[];
            return BinaryData.Deserialize(o);
        }

        public override void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            ((IDbDataParameter)cmd.Parameters[index]).Size = int.MaxValue;

            NHibernateUtil.BinaryBlob.NullSafeSet(cmd, BinaryData.Serialize(value), index);
        }

        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.BinaryBlob.SqlType }; }
        }
    }
    public static class BinaryData
    {
        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();
        public static bool CanSerialize(object value)
        {
            try
            {
                Serialize(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static byte[] Serialize(object value)
        {
            using (var memoryStream = new MemoryStream())
            {
                BinaryFormatter.Serialize(memoryStream, value);
                return memoryStream.GetBuffer();
            }
        }

        public static object Deserialize(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                try
                {
                    var deserialize = BinaryFormatter.Deserialize(memoryStream);
                    return deserialize;
                }
                catch
                {
                    return null;
                }
            }
        }
    }

}