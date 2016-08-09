using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class PasswordEncryptionManager : IPasswordEncryptionManager
    {
        private readonly IHashAlgorithmProvider _hashAlgorithmProvider;
        private readonly IUserManagementService _userManagementService;

        public PasswordEncryptionManager(IHashAlgorithmProvider hashAlgorithmProvider, IUserManagementService userManagementService)
        {
            _hashAlgorithmProvider = hashAlgorithmProvider;
            _userManagementService = userManagementService;
        }

        public void UpdateEncryption(User user, string password)
        {
            SetPassword(user, password);
            _userManagementService.SaveUser(user);
        }

        public virtual void SetPassword(User user, string password)
        {
            var salt = CreateSalt(64);

            var hashAlgorithm = _hashAlgorithmProvider.GetHashAlgorithm(null);
            user.PasswordHash = hashAlgorithm.GenerateSaltedHash(GetBytes(password), salt);
            user.PasswordSalt = salt;
            user.CurrentEncryption = null;
        }

        public bool ValidateUser(User user, string password)
        {
            var hashAlgorithm = _hashAlgorithmProvider.GetHashAlgorithm(user.CurrentEncryption);
            return CompareByteArrays(user.PasswordHash, hashAlgorithm.GenerateSaltedHash(GetBytes(password), user.PasswordSalt));
        }

        private static byte[] GetBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        private static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            return array1.Length == array2.Length && !array1.Where((t, i) => t != array2[i]).Any();
        }

        private static byte[] CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }
    }
}