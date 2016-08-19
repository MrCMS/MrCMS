using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserManagementServiceTests : InMemoryDatabaseTest
    {
        private ISession _session;
        private UserManagementService _userManagementService;

        public UserManagementServiceTests()
        {
            _session = Session;
            _userManagementService = new UserManagementService(_session);
        }

        [Fact]
        public void UserService_AddUser_SavesAUserToSession()
        {
            _session = A.Fake<ISession>();
            _userManagementService = new UserManagementService(_session);
            var user = new User();

            _userManagementService.AddUser(user);

            A.CallTo(() => _session.Save(user)).MustHaveHappened();
        }

        [Fact]
        public void UserService_SaveUser_UpdatesAUser()
        {
            _session = A.Fake<ISession>();
            _userManagementService = new UserManagementService(_session);
            var user = new User();

            _userManagementService.SaveUser(user);

            A.CallTo(() => _session.Update(user)).MustHaveHappened();
        }

        [Fact]
        public void UserService_GetUser_ShouldReturnCorrectUser()
        {
            var user = new User {FirstName = "Test", LastName = "User"};
            Session.Transact(session => session.SaveOrUpdate(user));

            var loadedUser = _userManagementService.GetUser(user.Id);

            loadedUser.Should().BeSameAs(user);
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
                i =>
                    Session.Transact(
                        session => session.SaveOrUpdate(new User {FirstName = "Test " + i, LastName = "User"})));

            var users = _userManagementService.GetAllUsersPaged(1);

            users.Count.Should().Be(10);
            users.PageCount.Should().Be(2);
        }

        [Fact]
        public void UserService_DeleteUser_ShouldRemoveAUser()
        {
            var user = new User();
            Session.Transact(session => session.Save(user));

            _userManagementService.DeleteUser(user);

            Session.QueryOver<User>().RowCount().Should().Be(0);
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnTrueIfThereAreNoOtherUsers()
        {
            _userManagementService.IsUniqueEmail("test@example.com").Should().BeTrue();
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnFalseIfThereIsAnotherUserWithTheSameEmail()
        {
            Session.Transact(session => session.Save(new User {Email = "test@example.com"}));
            _userManagementService.IsUniqueEmail("test@example.com").Should().BeFalse();
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnTrueIfTheIdPassedAlongWithTheSavedEmailIsThatOfTheSameUser()
        {
            var user = new User {Email = "test@example.com"};
            Session.Transact(session => session.Save(user));
            _userManagementService.IsUniqueEmail("test@example.com", user.Id).Should().BeTrue();
        }
    }
}