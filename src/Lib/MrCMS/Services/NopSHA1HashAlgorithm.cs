using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MrCMS.Services
{
    public class NopSHA1HashAlgorithm : IHashAlgorithm
    {
        public byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            var stringValue = string.Format("{0}{1}", Encoding.UTF8.GetString(plainText), Encoding.UTF8.GetString(salt));
            var algorithm = SHA1.Create();
            var data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(stringValue));
            return
                Encoding.UTF8.GetBytes(data.Aggregate("", (current, t) => current + t.ToString("x2").ToUpperInvariant()));
        }

        public string Type
        {
            get { return "NopSHA1"; }
        }
    }
}