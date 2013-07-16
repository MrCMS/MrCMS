using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IAuthorisationService
    {
        void ValidatePassword(string password, string confirmation);
        void SetPassword(User user, string password, string confirmation);
        bool ValidateUser(User user, string password);
        void SetAuthCookie(string email, bool rememberMe);
        void Logout();
    }

    public class AuthorisationService : IAuthorisationService
    {
        public void ValidatePassword(string password, string confirmation)
        {
            if (password != confirmation)
            {
                throw new InvalidPasswordException("The passwords you have chosen do not match");
            }
        }

        public void SetPassword(User user, string password, string confirmation)
        {
            ValidatePassword(password, confirmation);

            var salt = CreateSalt(64);

            user.PasswordHash = GenerateSaltedHash(GetBytes(password), salt);
            user.PasswordSalt = salt;
        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA512Managed();

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

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        private static byte[] GetBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        private static byte[] CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }

        public bool ValidateUser(User user, string password)
        {
            return CompareByteArrays(user.PasswordHash, GenerateSaltedHash(GetBytes(password), user.PasswordSalt));
        }

        private static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            return array1.Length == array2.Length && !array1.Where((t, i) => t != array2[i]).Any();
        }

        public void SetAuthCookie(string email, bool rememberMe)
        {
            FormsAuthentication.SetAuthCookie(email, rememberMe);
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}