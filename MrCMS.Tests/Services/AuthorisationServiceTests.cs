using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class AuthorisationServiceTests
    {
        private static AuthorisationService GetAuthorisationService()
        {
            var authorisationService = new AuthorisationService();
            return authorisationService;
        }

        [Fact]
        public void AuthorisationService_ValidatePassword_ShouldThrowOnDifferentPasswords()
        {
            var authorisationService = GetAuthorisationService();

            authorisationService.Invoking(service => service.ValidatePassword("password", "differentpassword")).
                ShouldThrow<InvalidPasswordException>();
        }

        [Fact]
        public void AuthorisationService_ValidatePassword_ReturnsTrueIfPasswordsAreTheSame()
        {
            var authorisationService = GetAuthorisationService();

            authorisationService.Invoking(service => service.ValidatePassword("password", "password")).ShouldNotThrow();
        }

        [Fact]
        public void AuthorisationService_SetPassword_WithInvalidPasswordShouldThrowException()
        {
            var authorisationService = GetAuthorisationService();

            var user = new User();

            authorisationService.Invoking(service => service.SetPassword(user,"password", "differentpassword")).
                ShouldThrow<InvalidPasswordException>();
            
        }

        [Fact]
        public void AuthorisationService_SetPassword_WithSamePasswordsShouldAssignThePasswordSaltAndHashOfTheUser()
        {
            var authorisationService = GetAuthorisationService();

            var user = new User();

            authorisationService.SetPassword(user, "password", "password");

            user.PasswordHash.Should().NotBeEmpty();
            user.PasswordSalt.Should().NotBeEmpty();
        }

        [Fact]
        public void AuthorisationService_ValidateUser_WithIncorrectPasswordShouldReturnFalse()
        {
            var authorisationService = GetAuthorisationService();
            var user = new User();
            authorisationService.SetPassword(user, "password", "password");

            var result = authorisationService.ValidateUser(user, "incorrectPassword");

            result.Should().BeFalse();
        }

        [Fact]
        public void AuthorisationService_ValidateUser_WithCorrectPasswordShouldReturnTrue()
        {
            var authorisationService = GetAuthorisationService();
            var user = new User();
            authorisationService.SetPassword(user, "password", "password");

            var result = authorisationService.ValidateUser(user, "password");

            result.Should().BeTrue();
        }
    }
}