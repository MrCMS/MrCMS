using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.TestSupport;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class UserLookupTests : MrCMSTest
    {
        private readonly UserLookup _userService;
        private IGetDateTimeNow _getDateTimeNow;
        private IGlobalRepository<User> _repository;

        public UserLookupTests()
        {
            _getDateTimeNow = A.Fake<IGetDateTimeNow>();
            _repository = A.Fake<IGlobalRepository<User>>();
            _userService = new UserLookup(_repository, new List<IExternalUserSource>(), _getDateTimeNow);
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
            var user2 = new User { FirstName = "Test", LastName = "User2", Email = "test2@example.com" };
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(user, user2);

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
            var dateTime = DateTime.UtcNow;
            A.CallTo(() => _getDateTimeNow.UtcNow).Returns(dateTime);
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                ResetPasswordGuid = resetPasswordGuid,
                ResetPasswordExpiry = dateTime.AddDays(-2)
            };
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(user);

            _userService.GetUserByResetGuid(resetPasswordGuid).Should().BeNull();
        }

        [Fact]
        public void UserService_GetUserByResetGuid_ValidGuidAndExpiryInTheFutureReturnsUser()
        {
            var resetPasswordGuid = Guid.NewGuid();
            var dateTime = DateTime.UtcNow;
            A.CallTo(() => _getDateTimeNow.UtcNow).Returns(dateTime);
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                ResetPasswordGuid = resetPasswordGuid,
                ResetPasswordExpiry = dateTime.AddDays(1)
            };
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(user);

            _userService.GetUserByResetGuid(resetPasswordGuid).Should().Be(user);
        }

        [Fact]
        public void UserService_GetCurrentUser_HttpContextUserIsNullReturnsNull()
        {
            var httpContextBase = new DefaultHttpContext {User = null};

            _userService.GetCurrentUser(httpContextBase).Should().BeNull();
        }

        [Fact]
        public void UserService_GetCurrentUser_HttpContextUserHasIdentityGetByEmail()
        {
            var httpContextBase = new DefaultHttpContext {User = new ClaimsPrincipal(new List<ClaimsIdentity>
            {
                new GenericIdentity("test@example.com")
            })};
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
            };
            A.CallTo(() => _repository.Query()).ReturnsAsAsyncQueryable(user);

            _userService.GetCurrentUser(httpContextBase).Should().Be(user);
        }

    }
}