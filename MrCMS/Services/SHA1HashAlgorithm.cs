using System.Security.Cryptography;

namespace MrCMS.Services
{
    public class SHA1HashAlgorithm : IHashAlgorithm
    {
        private readonly SHA1Managed _algorithm;

        public SHA1HashAlgorithm()
        {

            _algorithm = new SHA1Managed();
        }

        public byte[] ComputeHash(byte[] data)
        {
            return _algorithm.ComputeHash(data);
        }
    }
}