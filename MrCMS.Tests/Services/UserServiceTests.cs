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
using MrCMS.Settings;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserServiceTests : InMemoryDatabaseTest
    {
        private static ISiteService siteService;

        private static UserService GetUserService(ISession session = null)
        {
            siteService = A.Fake<ISiteService>();
            var userService = new UserService(session ?? Session, siteService);
            return userService;
        }

        [Fact]
        public void UserService_AddUser_SavesAUserToSession()
        {
            var session = A.Fake<ISession>();
            var userService = GetUserService(session);

            var user = new User();
            userService.AddUser(user);

            A.CallTo(() => session.Save(user)).MustHaveHappened();
        }

        [Fact]
        public void UserService_AddUser_AddsTheCurrentSiteToTheUsersSites()
        {
            var session = A.Fake<ISession>();
            var userService = GetUserService(session);
            var site = new Site();
            A.CallTo(() => siteService.GetCurrentSite()).Returns(site);

            var user = new User();
            userService.AddUser(user);

            user.Sites.Should().Contain(site);
        }

        [Fact]
        public void UserService_AddUser_AddsTheUserToTheSitesUsers()
        {
            var session = A.Fake<ISession>();
            var userService = GetUserService(session);
            var site = new Site();
            A.CallTo(() => siteService.GetCurrentSite()).Returns(site);

            var user = new User();
            userService.AddUser(user);

            site.Users.Should().Contain(user);
        }

        [Fact]
        public void UserService_SaveUser_UpdatesAUser()
        {
            var session = A.Fake<ISession>();
            var userService = GetUserService(session);

            var user = new User();
            userService.SaveUser(user);

            A.CallTo(() => session.Update(user)).MustHaveHappened();
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

        [Fact]
        public void UserService_GetUserByEmail_ReturnsNullWhenNoUserAvailable()
        {
            var userService = GetUserService();

            userService.GetUserByEmail("test@example.com").Should().BeNull();
        }

        [Fact]
        public void UserService_GetUserByEmail_WithValidEmailReturnsTheCorrectUser()
        {
            var userService = GetUserService();

            var user = new User { FirstName = "Test", LastName = "User", Email = "test@example.com" };
            Session.Transact(session => Session.Save(user));
            var user2 = new User { FirstName = "Test", LastName = "User2", Email = "test2@example.com" };
            Session.Transact(session => Session.Save(user2));

            userService.GetUserByEmail("test2@example.com").Should().Be(user2);
        }

        [Fact]
        public void UserService_GetUserByResetGuid_ReturnsNullForInvalidGuid()
        {
            var userService = GetUserService();

            userService.GetUserByResetGuid(Guid.Empty).Should().BeNull();
        }

        [Fact]
        public void UserService_GetUserByResetGuid_ValidGuidButExpiryPassedReturnsNull()
        {
            var userService = GetUserService();

            var resetPasswordGuid = Guid.NewGuid();
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                ResetPasswordGuid = resetPasswordGuid,
                ResetPasswordExpiry = DateTime.UtcNow.AddDays(-2)
            };
            Session.Transact(session => Session.Save(user));

            userService.GetUserByResetGuid(resetPasswordGuid).Should().BeNull();
        }

        [Fact]
        public void UserService_GetUserByResetGuid_ValidGuidAndExpiryInTheFutureReturnsUser()
        {
            var userService = GetUserService();

            var resetPasswordGuid = Guid.NewGuid();
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                ResetPasswordGuid = resetPasswordGuid,
                ResetPasswordExpiry = DateTime.UtcNow.AddDays(1)
            };
            Session.Transact(session => Session.Save(user));

            userService.GetUserByResetGuid(resetPasswordGuid).Should().Be(user);
        }

        [Fact]
        public void UserService_GetCurrentUser_HttpContextUserIsNullReturnsNull()
        {
            var userService = GetUserService();
            var httpContextBase = A.Fake<HttpContextBase>();
            A.CallTo(() => httpContextBase.User).Returns(null);

            userService.GetCurrentUser(httpContextBase).Should().BeNull();
        }

        [Fact]
        public void UserService_GetCurrentUser_HttpContextUserHasIdentityGetByEmail()
        {
            var userService = GetUserService();
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

            userService.GetCurrentUser(httpContextBase).Should().Be(user);
        }
    }
}