using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class PasswordManagementService : IPasswordManagementService
    {
        private readonly IPasswordEncryptionManager _passwordEncryptionManager;

        public PasswordManagementService(IPasswordEncryptionManager passwordEncryptionManager)
        {
            _passwordEncryptionManager = passwordEncryptionManager;
        }

        public bool ValidatePassword(string password, string confirmation)
        {
            return (password == confirmation);
        }

        public void SetPassword(User user, string password, string confirmation)
        {
            if (ValidatePassword(password, confirmation))
            {
                _passwordEncryptionManager.SetPassword(user, password);
            }
        }

        public bool ValidateUser(User user, string password)
        {
            var compareByteArrays = _passwordEncryptionManager.ValidateUser(user, password);
            if (compareByteArrays && !string.IsNullOrWhiteSpace(user.CurrentEncryption))
            {
                _passwordEncryptionManager.UpdateEncryption(user, password);
            }
            return compareByteArrays;
        }
    }

    public class PasswordEncryptionManager : IPasswordEncryptionManager
    {
        private readonly IHashAlgorithmProvider _hashAlgorithmProvider;
        private readonly IUserService _userService;

        public PasswordEncryptionManager(IHashAlgorithmProvider hashAlgorithmProvider, IUserService userService)
        {
            _hashAlgorithmProvider = hashAlgorithmProvider;
            _userService = userService;
        }

        public void UpdateEncryption(User user, string password)
        {
            user.CurrentEncryption = null;
            SetPassword(user, password);
            _userService.SaveUser(user);
        }

        public virtual void SetPassword(User user, string password)
        {
            var salt = CreateSalt(64);

            var hashAlgorithm = _hashAlgorithmProvider.GetHashAlgorithm(user.CurrentEncryption);
            user.PasswordHash = hashAlgorithm.GenerateSaltedHash(GetBytes(password), salt);
            user.PasswordSalt = salt;
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