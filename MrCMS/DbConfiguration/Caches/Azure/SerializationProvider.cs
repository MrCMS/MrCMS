using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MrCMS.DbConfiguration.Caches.Azure
{
    public class SerializationProvider : ISerializationProvider
    {
        private readonly IFormatter _formatter;

        public SerializationProvider()
        {
            _formatter = new BinaryFormatter();
        }

        public byte[] Serialize(object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                _formatter.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public object Deserialize(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);

                return _formatter.Deserialize(stream);
            }
        }
    }
}