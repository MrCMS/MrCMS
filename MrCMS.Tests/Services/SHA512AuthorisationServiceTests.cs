using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class AuthorisationServiceTests
    {
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly AuthorisationService _authorisationService;

        public AuthorisationServiceTests()
        {
            _hashAlgorithm = A.Fake<IHashAlgorithm>();
            _authorisationService = new AuthorisationService(_hashAlgorithm);
        }

        [Fact]
        public void AuthorisationService_ValidatePassword_ShouldThrowOnDifferentPasswords()
        {
            _authorisationService.Invoking(service => service.ValidatePassword("password", "differentpassword")).
                ShouldThrow<InvalidPasswordException>();
        }

        [Fact]
        public void AuthorisationService_ValidatePassword_ReturnsTrueIfPasswordsAreTheSame()
        {
            _authorisationService.Invoking(service => service.ValidatePassword("password", "password")).ShouldNotThrow();
        }

        [Fact]
        public void AuthorisationService_SetPassword_WithInvalidPasswordShouldThrowException()
        {
            var user = new User();

            _authorisationService.Invoking(service => service.SetPassword(user, "password", "differentpassword")).
                ShouldThrow<InvalidPasswordException>();

        }

        [Fact]
        public void AuthorisationService_SetPassword_WithSamePasswordsShouldAssignThePasswordSaltAndHashOfTheUser()
        {
            var user = new User();
            A.CallTo(() => _hashAlgorithm.ComputeHash(A<byte[]>._)).Returns(new byte[64]);

            _authorisationService.SetPassword(user, "password", "password");

            user.PasswordHash.Should().NotBeEmpty();
            user.PasswordSalt.Should().NotBeEmpty();
        }

        [Fact]
        public void AuthorisationService_ValidateUser_WithCorrectPasswordShouldReturnTrue()
        {
            var user = new User();
            _authorisationService.SetPassword(user, "password", "password");

            var result = _authorisationService.ValidateUser(user, "password");

            result.Should().BeTrue();
        }
    }
}