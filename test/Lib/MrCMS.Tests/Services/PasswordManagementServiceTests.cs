using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class PasswordManagementServiceTests
    {
        private readonly PasswordManagementService _passwordManagementService;
        private readonly IPasswordEncryptionManager _passwordEncryptionManager;

        public PasswordManagementServiceTests()
        {
            _passwordEncryptionManager = A.Fake<IPasswordEncryptionManager>();
            _passwordManagementService = new PasswordManagementService(new List<IExternalUserSource>(), _passwordEncryptionManager);
        }

        [Fact]
        public void PasswordManagementService_ValidatePassword_ShouldBeFalse()
        {
            _passwordManagementService.ValidatePassword("password", "differentpassword").Should().BeFalse();
        }

        [Fact]
        public void PasswordManagementService_ValidatePassword_ReturnsTrueIfPasswordsAreTheSame()
        {
            _passwordManagementService.ValidatePassword("password", "password").Should().BeTrue();
        }

        [Fact]
        public void PasswordManagementService_SetPassword_WithInvalidPasswordShouldNotSetValues()
        {
            var user = new User();

            _passwordManagementService.SetPassword(user, "password", "differentpassword");

            A.CallTo(() => _passwordEncryptionManager.SetPassword(user, "password")).MustNotHaveHappened();
        }

        [Fact]
        public void PasswordManagementService_SetPassword_WithSamePasswordsShouldCallSetPasswordIfTheyMatch()
        {
            var user = new User();

            _passwordManagementService.SetPassword(user, "password", "password");

            A.CallTo(() => _passwordEncryptionManager.SetPassword(user, "password")).MustHaveHappened();
        }

        [Fact]
        public async Task PasswordManagementService_ValidateUser_WithCorrectPasswordShouldReturnTrue()
        {
            var user = new User();
            A.CallTo(() => _passwordEncryptionManager.ValidateUser(user, "password")).Returns(true);

            var result =await  _passwordManagementService.ValidateUser(user, "password");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PasswordManagementService_ValidateUser_WithIncorrectPasswordShouldReturnTrue()
        {
            var user = new User();
            A.CallTo(() => _passwordEncryptionManager.ValidateUser(user, "password")).Returns(false);

            var result = await _passwordManagementService.ValidateUser(user, "password");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task PasswordManagementService_ValidateUser_ShouldCallUpdateEncryptionIfItIsSetAgainstTheUserAndItIsValidated()
        {
            var user = new User { CurrentEncryption = "SHA1" };
            A.CallTo(() => _passwordEncryptionManager.ValidateUser(user, "password")).Returns(true);

            await _passwordManagementService.ValidateUser(user, "password");

            A.CallTo(() => _passwordEncryptionManager.UpdateEncryption(user, "password")).MustHaveHappened();
        }

        [Fact]
        public async Task PasswordManagementService_ValidateUser_ShouldNotCallUpdateEncryptionIfItIsSetAgainstTheUserAndItIsNotValidated()
        {
            var user = new User { CurrentEncryption = "SHA1" };
            A.CallTo(() => _passwordEncryptionManager.ValidateUser(user, "password")).Returns(false);

            await _passwordManagementService.ValidateUser(user, "password");

            A.CallTo(() => _passwordEncryptionManager.UpdateEncryption(user, "password")).MustNotHaveHappened();
        }
    }
}