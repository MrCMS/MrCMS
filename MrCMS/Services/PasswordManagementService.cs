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
}