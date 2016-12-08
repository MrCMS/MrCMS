using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class PasswordManagementService : IPasswordManagementService
    {
        private readonly IPasswordEncryptionManager _passwordEncryptionManager;
        private readonly Dictionary<string, IExternalUserSource> _userSources;

        public PasswordManagementService(IEnumerable<IExternalUserSource> userSources, IPasswordEncryptionManager passwordEncryptionManager)
        {
            _passwordEncryptionManager = passwordEncryptionManager;
            _userSources = userSources.GroupBy(x => x.Name).ToDictionary(x => x.Key, grouping => grouping.First());
        }

        public bool ValidatePassword(string password, string confirmation)
        {
            return password == confirmation;
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
            if (!string.IsNullOrWhiteSpace(user.Source) && _userSources.ContainsKey(user.Source))
                return _userSources[user.Source].ValidateUser(user, password);

            var compareByteArrays = _passwordEncryptionManager.ValidateUser(user, password);
            if (compareByteArrays && !string.IsNullOrWhiteSpace(user.CurrentEncryption))
            {
                _passwordEncryptionManager.UpdateEncryption(user, password);
            }
            return compareByteArrays;
        }
    }
}