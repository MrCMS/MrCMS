using System.Security.Cryptography;

namespace MrCMS.Services
{
    public class SHA512HashAlgorithm : IHashAlgorithm
    {
        private readonly SHA512Managed _algorithm;

        public SHA512HashAlgorithm()
        {

            _algorithm = new SHA512Managed();
        }

        public byte[] ComputeHash(byte[] data)
        {
            return _algorithm.ComputeHash(data);
        }
    }
}