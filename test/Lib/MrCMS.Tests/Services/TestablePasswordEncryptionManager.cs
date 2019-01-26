using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Tests.Services
{
    public class TestablePasswordEncryptionManager : PasswordEncryptionManager
    {
        private bool _setPasswordCalled;

        public TestablePasswordEncryptionManager(IHashAlgorithmProvider hashAlgorithmProvider, IUserManagementService userService)
            : base(hashAlgorithmProvider, userService)
        {
        }

        public override void SetPassword(User user, string password)
        {
            base.SetPassword(user, password);
            _setPasswordCalled = true;
        }
        public bool SetPasswordCalled
        {
            get { return _setPasswordCalled; }
        }
    }
}