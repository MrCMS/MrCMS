using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Tests.TestSupport;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserManagementServiceTests
    {
        private UserManagementService _userManagementService;
        private IRepository<User> _userRepository = new InMemoryRepository<User>();

        public UserManagementServiceTests()
        {
            _userManagementService = new UserManagementService(_userRepository);
        }

        [Fact]
        public void UserService_GetUserDoesNotExist_ShouldReturnNull()
        {
            var loadedUser = _userManagementService.GetUser(-1);

            loadedUser.Should().BeNull();
        }

        [Fact]
        public void UserService_GetAllUsersPaged_ShouldReturnTheCollectionOfUsersPaged()
        {
            Enumerable.Range(1, 15).ForEach(
                i => _userRepository.Add(new User { FirstName = "Test " + i, LastName = "User" }));

            var users = _userManagementService.GetAllUsersPaged(1);

            users.Count.Should().Be(10);
            users.PageCount.Should().Be(2);
        }


        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnTrueIfThereAreNoOtherUsers()
        {
            _userManagementService.IsUniqueEmail("test@example.com").Should().BeTrue();
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnFalseIfThereIsAnotherUserWithTheSameEmail()
        {
            _userRepository.Add(new User { Email = "test@example.com" });
            _userManagementService.IsUniqueEmail("test@example.com").Should().BeFalse();
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnTrueIfTheIdPassedAlongWithTheSavedEmailIsThatOfTheSameUser()
        {
            var user = new User { Email = "test@example.com" };
            _userRepository.Add(user);
            _userManagementService.IsUniqueEmail("test@example.com", user.Id).Should().BeTrue();
        }
    }
}