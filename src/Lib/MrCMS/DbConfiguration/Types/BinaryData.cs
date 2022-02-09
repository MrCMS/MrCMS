using System;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class BinaryData<T> : BaseImmutableUserType<T> where T : class, new()
    {
        public override SqlType[] SqlTypes
        {
            get { return new[] { NHibernateUtil.BinaryBlob.SqlType }; }
        }

        public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var o = NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0], session, owner) as byte[];
            return BinaryData.Deserialize<T>(o);
        }

        public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            cmd.Parameters[index].Size = int.MaxValue;
            NHibernateUtil.BinaryBlob.NullSafeSet(cmd, BinaryData.Serialize(value), index, session);
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
            if (value == null)
                return null;
            using var memoryStream = new MemoryStream();
            BinaryFormatter.Serialize(memoryStream, value);
            return memoryStream.ToArray();
        }

        public static T Deserialize<T>(byte[] bytes) where T : class, new()
        {
            if (bytes == null)
                return new T();
            try
            {
                using (var memoryStream = new MemoryStream(bytes))
                {
                    object deserialize = BinaryFormatter.Deserialize(memoryStream);
                    return deserialize as T ?? new T();
                }
            }
            catch
            {
                return new T();
            }
        }
    }
}