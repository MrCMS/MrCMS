using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserServiceTests : InMemoryDatabaseTest
    {
        private static UserService GetUserService()
        {
            var authorisationService = A.Fake<IAuthorisationService>();
            var userService = new UserService(new SiteSettings(), Session, authorisationService);
            return userService;
        }

        [Fact]
        public void UserService_SaveUser_AddsAUserToTHeDb()
        {
            var userService = GetUserService();

            userService.SaveUser(new User());

            Session.QueryOver<User>().RowCount().Should().Be(1);
        }

        [Fact]
        public void UserService_GetUser_ShouldReturnCorrectUser()
        {
            var userService = GetUserService();
            var user = new User { FirstName = "Test", LastName = "User" };
            Session.Transact(session => session.SaveOrUpdate(user));

            var loadedUser = userService.GetUser(user.Id);

            loadedUser.Should().BeSameAs(user);
        }

        [Fact]
        public void UserService_GetUserDoesNotExist_ShouldReturnNull()
        {
            var userService = GetUserService();

            var loadedUser = userService.GetUser(-1);

            loadedUser.Should().BeNull();
        }

        [Fact]
        public void UserService_GetAllUsers_ShouldReturnTheCollectionOfUsers()
        {
            var userService = GetUserService();

            Enumerable.Range(1, 15).ForEach(
                i =>
                Session.Transact(session => session.SaveOrUpdate(new User { FirstName = "Test " + i, LastName = "User" })));

            var users = userService.GetAllUsers();

            users.Should().HaveCount(15);
        }
    }
}