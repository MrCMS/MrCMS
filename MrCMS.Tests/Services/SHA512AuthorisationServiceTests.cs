using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class SHA512AuthorisationServiceTests
    {
        private static SHA512AuthorisationService GetSHA512AuthorisationService()
        {
            var authorisationService = new SHA512AuthorisationService();
            return authorisationService;
        }

        [Fact]
        public void SHA512AuthorisationService_ValidatePassword_ShouldThrowOnDifferentPasswords()
        {
            var authorisationService = GetSHA512AuthorisationService();

            authorisationService.Invoking(service => service.ValidatePassword("password", "differentpassword")).
                ShouldThrow<InvalidPasswordException>();
        }

        [Fact]
        public void SHA512AuthorisationService_ValidatePassword_ReturnsTrueIfPasswordsAreTheSame()
        {
            var authorisationService = GetSHA512AuthorisationService();

            authorisationService.Invoking(service => service.ValidatePassword("password", "password")).ShouldNotThrow();
        }

        [Fact]
        public void SHA512AuthorisationService_SetPassword_WithInvalidPasswordShouldThrowException()
        {
            var authorisationService = GetSHA512AuthorisationService();

            var user = new User();

            authorisationService.Invoking(service => service.SetPassword(user, "password", "differentpassword")).
                ShouldThrow<InvalidPasswordException>();

        }

        [Fact]
        public void SHA512AuthorisationService_SetPassword_WithSamePasswordsShouldAssignThePasswordSaltAndHashOfTheUser()
        {
            var authorisationService = GetSHA512AuthorisationService();

            var user = new User();

            authorisationService.SetPassword(user, "password", "password");

            user.PasswordHash.Should().NotBeEmpty();
            user.PasswordSalt.Should().NotBeEmpty();
        }

        [Fact]
        public void SHA512AuthorisationService_ValidateUser_WithIncorrectPasswordShouldReturnFalse()
        {
            var authorisationService = GetSHA512AuthorisationService();
            var user = new User();
            authorisationService.SetPassword(user, "password", "password");

            var result = authorisationService.ValidateUser(user, "incorrectPassword");

            result.Should().BeFalse();
        }

        [Fact]
        public void SHA512AuthorisationService_ValidateUser_WithCorrectPasswordShouldReturnTrue()
        {
            var authorisationService = GetSHA512AuthorisationService();
            var user = new User();
            authorisationService.SetPassword(user, "password", "password");

            var result = authorisationService.ValidateUser(user, "password");

            result.Should().BeTrue();
        }
    }
}