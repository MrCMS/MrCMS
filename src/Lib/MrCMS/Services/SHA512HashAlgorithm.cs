using System.Security.Cryptography;

namespace MrCMS.Services
{
    public class SHA512HashAlgorithm : IHashAlgorithm
    {
        public const string Name = "SHA512";
        private readonly SHA512Managed _algorithm = new();
        
        public byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            var plainTextWithSaltBytes =
                new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return _algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        public string Type => Name;
    }

}