using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace MrCMS.Services
{
    public class NopSHA1HashAlgorithm : IHashAlgorithm
    {
        public byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            return Encoding.UTF8.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(string.Format("{0}{1}", Encoding.UTF8.GetString(plainText), Encoding.UTF8.GetString(salt)), "SHA1"));
        }

        public string Type { get { return "NopSHA1"; } }
    }
}