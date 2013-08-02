using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserServiceTests : InMemoryDatabaseTest
    {
        private static ISiteService _siteService;
        private UserService _userService;
        private ISession _session;

        public UserServiceTests()
        {
            _session = Session;
            _siteService = A.Fake<ISiteService>();
            _userService = new UserService(_session, _siteService);
        }

        [Fact]
        public void UserService_AddUser_SavesAUserToSession()
        {
            _session = A.Fake<ISession>();
            _userService = new UserService(_session, _siteService);
            var user = new User();

            _userService.AddUser(user);

            A.CallTo(() => _session.Save(user)).MustHaveHappened();
        }

        [Fact]
        public void UserService_SaveUser_UpdatesAUser()
        {
            _session = A.Fake<ISession>();
            _userService = new UserService(_session, _siteService);
            var user = new User();

            _userService.SaveUser(user);

            A.CallTo(() => _session.Update(user)).MustHaveHappened();
        }

        [Fact]
        public void UserService_GetUser_ShouldReturnCorrectUser()
        {
            var user = new User { FirstName = "Test", LastName = "User" };
            Session.Transact(session => session.SaveOrUpdate(user));

            var loadedUser = _userService.GetUser(user.Id);

            loadedUser.Should().BeSameAs(user);
        }

        [Fact]
        public void UserService_GetUserDoesNotExist_ShouldReturnNull()
        {
            var loadedUser = _userService.GetUser(-1);

            loadedUser.Should().BeNull();
        }

        [Fact]
        public void UserService_GetAllUsers_ShouldReturnTheCollectionOfUsers()
        {
            Enumerable.Range(1, 15).ForEach(
                i =>
                Session.Transact(session => session.SaveOrUpdate(new User { FirstName = "Test " + i, LastName = "User" })));

            var users = _userService.GetAllUsers();

            users.Should().HaveCount(15);
        }

        [Fact]
        public void UserService_GetAllUsersPaged_ShouldReturnTheCollectionOfUsersPaged()
        {
            Enumerable.Range(1, 15).ForEach(
                i =>
                Session.Transact(session => session.SaveOrUpdate(new User { FirstName = "Test " + i, LastName = "User" })));

            var users = _userService.GetAllUsersPaged(1);

            users.Count.Should().Be(10);
            users.PageCount.Should().Be(2);
        }

        [Fact]
        public void UserService_GetUserByEmail_ReturnsNullWhenNoUserAvailable()
        {
            _userService.GetUserByEmail("test@example.com").Should().BeNull();
        }

        [Fact]
        public void UserService_GetUserByEmail_WithValidEmailReturnsTheCorrectUser()
        {
            var user = new User { FirstName = "Test", LastName = "User", Email = "test@example.com" };
            Session.Transact(session => Session.Save(user));
            var user2 = new User { FirstName = "Test", LastName = "User2", Email = "test2@example.com" };
            Session.Transact(session => Session.Save(user2));

            _userService.GetUserByEmail("test2@example.com").Should().Be(user2);
        }

        [Fact]
        public void UserService_GetUserByResetGuid_ReturnsNullForInvalidGuid()
        {
            _userService.GetUserByResetGuid(Guid.Empty).Should().BeNull();
        }

        [Fact]
        public void UserService_GetUserByResetGuid_ValidGuidButExpiryPassedReturnsNull()
        {
            var resetPasswordGuid = Guid.NewGuid();
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                ResetPasswordGuid = resetPasswordGuid,
                ResetPasswordExpiry = CurrentRequestData.Now.AddDays(-2)
            };
            Session.Transact(session => Session.Save(user));

            _userService.GetUserByResetGuid(resetPasswordGuid).Should().BeNull();
        }

        [Fact]
        public void UserService_GetUserByResetGuid_ValidGuidAndExpiryInTheFutureReturnsUser()
        {
            var resetPasswordGuid = Guid.NewGuid();
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                ResetPasswordGuid = resetPasswordGuid,
                ResetPasswordExpiry = CurrentRequestData.Now.AddDays(1)
            };
            Session.Transact(session => Session.Save(user));

            _userService.GetUserByResetGuid(resetPasswordGuid).Should().Be(user);
        }

        [Fact]
        public void UserService_GetCurrentUser_HttpContextUserIsNullReturnsNull()
        {
            var httpContextBase = A.Fake<HttpContextBase>();
            A.CallTo(() => httpContextBase.User).Returns(null);

            _userService.GetCurrentUser(httpContextBase).Should().BeNull();
        }

        [Fact]
        public void UserService_GetCurrentUser_HttpContextUserHasIdentityGetByEmail()
        {
            var httpContextBase = A.Fake<HttpContextBase>();
            var principal = A.Fake<IPrincipal>();
            var identity = A.Fake<IIdentity>();
            A.CallTo(() => identity.Name).Returns("test@example.com");
            A.CallTo(() => principal.Identity).Returns(identity);
            A.CallTo(() => httpContextBase.User).Returns(principal);
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
            };
            Session.Transact(session => Session.Save(user));

            _userService.GetCurrentUser(httpContextBase).Should().Be(user);
        }

        [Fact]
        public void UserService_DeleteUser_ShouldRemoveAUser()
        {
            var user = new User();
            Session.Transact(session => session.Save(user));

            _userService.DeleteUser(user);

            Session.QueryOver<User>().RowCount().Should().Be(0);
        }

        [Fact]
        public void UserService_DeleteUser_ShouldCallOnDeleteOnTheUser()
        {
            var user = A.Fake<User>();
            _session = A.Fake<ISession>();
            _userService = new UserService(_session, _siteService);

            _userService.DeleteUser(user);

            A.CallTo(() => user.OnDeleting(_session)).MustHaveHappened();
        }

        [Fact]
        public void UserService_ActiveUsers_ShouldReturnTheNumberOfUsersThatAreActiveSavedInTheSession()
        {
            Enumerable.Range(1, 10)
                      .Select(i => new User {IsActive = true})
                      .ForEach(user => Session.Transact(session => session.Save(user)));
            Enumerable.Range(1, 5).Select(i =>  new User {IsActive = false})
                      .ForEach(user => Session.Transact(session => session.Save(user)));

            _userService.ActiveUsers().Should().Be(10);
        }

        [Fact]
        public void UserService_NonActiveUsers_ShouldReturnTheNumberOfUsersThatAreNotActiveSavedInTheSession()
        {
            Enumerable.Range(1, 10)
                      .Select(i => new User {IsActive = true})
                      .ForEach(user => Session.Transact(session => session.Save(user)));
            Enumerable.Range(1, 5).Select(i =>  new User {IsActive = false})
                      .ForEach(user => Session.Transact(session => session.Save(user)));

            _userService.NonActiveUsers().Should().Be(5);
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnTrueIfThereAreNoOtherUsers()
        {
            _userService.IsUniqueEmail("test@example.com").Should().BeTrue();
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnFalseIfThereIsAnotherUserWithTheSameEmail()
        {
            Session.Transact(session => session.Save(new User {Email = "test@example.com"}));
            _userService.IsUniqueEmail("test@example.com").Should().BeFalse();
        }

        [Fact]
        public void UserService_IsUniqueEmail_ShouldReturnTrueIfTheIdPassedAlongWithTheSavedEmailIsThatOfTheSameUser()
        {
            var user = new User {Email = "test@example.com"};
            Session.Transact(session => session.Save(user));
            _userService.IsUniqueEmail("test@example.com",user.Id).Should().BeTrue();
        }
    }
}