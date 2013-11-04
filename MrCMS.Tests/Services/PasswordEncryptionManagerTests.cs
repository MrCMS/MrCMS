using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class PasswordEncryptionManagerTests
    {
        private readonly IHashAlgorithmProvider _hashAlgorithmProvider;
        private readonly IUserService _userService;
        private readonly PasswordEncryptionManager _passwordEncryptionManager;

        public PasswordEncryptionManagerTests()
        {
            _hashAlgorithmProvider = A.Fake<IHashAlgorithmProvider>();
            _userService = A.Fake<IUserService>();
            _passwordEncryptionManager = new PasswordEncryptionManager(_hashAlgorithmProvider, _userService);
        }

        [Fact]
        public void PasswordEncryptionManager_UpdateEncryption_ShouldResetCurrentEncryption()
        {
            var user = new User { CurrentEncryption = "SHA1" };

            _passwordEncryptionManager.UpdateEncryption(user, "password");

            user.CurrentEncryption.Should().BeNull();
        }

        [Fact]
        public void PasswordEncryptionManager_UpdateEncryption_ShouldCallSetPassword()
        {
            var user = new User { CurrentEncryption = "SHA1" };

            var testablePasswordEncryptionManager = new TestablePasswordEncryptionManager(_hashAlgorithmProvider, _userService);
            testablePasswordEncryptionManager.UpdateEncryption(user, "password");

            testablePasswordEncryptionManager.SetPasswordCalled.Should().BeTrue();
        }

        [Fact]
        public void PasswordEncryptionManager_UpdateEncryption_ShouldSaveUser()
        {
            var user = new User { CurrentEncryption = "SHA1" };

            _passwordEncryptionManager.UpdateEncryption(user, "password");

            A.CallTo(() => _userService.SaveUser(user)).MustHaveHappened();
        }

        [Fact]
        public void PasswordEncryptionManager_SetPassword_ShouldUpdatePasswordSaltAndHash()
        {
            var user = new User();
            A.CallTo(() => _hashAlgorithmProvider.GetHashAlgorithm(null)).Returns(new SHA512HashAlgorithm());

            _passwordEncryptionManager.SetPassword(user, "password");

            user.PasswordHash.Should().NotBeEmpty();
            user.PasswordSalt.Should().NotBeEmpty();
        }
    }
}