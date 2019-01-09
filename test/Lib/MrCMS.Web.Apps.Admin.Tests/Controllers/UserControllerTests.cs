using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.TestSupport;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using System.Collections.Generic;
using X.PagedList;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class UserControllerTests
    {
        public UserControllerTests()
        {
            _userSearchService = A.Fake<IUserSearchService>();
            _userService = A.Fake<IUserAdminService>();
            _roleService = A.Fake<IRoleService>();
            _passwordManagementService = A.Fake<IPasswordManagementService>();
            _getUserCultureOptions = A.Fake<IGetUserCultureOptions>();
            _userController = new UserController(_userService, _userSearchService, _roleService,
                _passwordManagementService, _getUserCultureOptions)
            {
                TempData = new MockTempDataDictionary()
            };
        }

        private readonly UserController _userController;
        private readonly IUserSearchService _userSearchService;
        private readonly IUserAdminService _userService;
        private readonly IRoleService _roleService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IGetUserCultureOptions _getUserCultureOptions;

        [Fact]
        public void UserController_AddGet_ShouldReturnAnAddUserModel()
        {
            var actionResult = _userController.Add();

            actionResult.As<PartialViewResult>().Model.Should().BeOfType<AddUserModel>();
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAViewResult()
        {
            var actionResult = _userController.Add();

            actionResult.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void UserController_AddPost_ShouldCallUserServiceSaveUser()
        {
            var user = new User();

            _userController.Add(user);

            A.CallTo(() => _userService.AddUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_AddPost_ShouldReturnRedirectEditForSavedUser()
        {
            var user = new User { Id = 123 };

            var result = _userController.Add(user);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnAViewResultWithThePassedUserAsAModel()
        {
            var user = new User();

            var result = _userController.Edit_Get(user);

            result.As<ViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnRedirectToIndexIfIdIsInvalid()
        {
            A.CallTo(() => _userService.GetUser(1)).Returns(null);

            var result = _userController.Edit_Get(null);

            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [Fact]
        public void UserController_EditGet_ShouldSetViewDataForAvailableRolesAndSites()
        {
            var user = new User();
            var roles = new List<UserRole>();
            A.CallTo(() => _roleService.GetAllRoles()).Returns(roles);

            _userController.Edit_Get(user);

            _userController.ViewData["AvailableRoles"].Should().Be(roles);
        }

        [Fact]
        public void UserController_EditPost_ShouldCallSaveUser()
        {
            var user = new UpdateUserModel();
            List<int> roles = new List<int>();

            _userController.Edit(user, roles);

            A.CallTo(() => _userService.SaveUser(user, roles)).MustHaveHappened();
        }

        [Fact]
        public void UserController_EditPost_ShouldReturnRedirectToEdit()
        {
            var user = new UpdateUserModel { Id = 123 };
            List<int> roles = new List<int>();

            var result = _userController.Edit(user, roles);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void UserController_Index_ShouldCallUserServiceGetUsersPaged()
        {
            var userSearchQuery = new UserSearchQuery();

            _userController.Index(userSearchQuery);

            A.CallTo(() => _userSearchService.GetUsersPaged(userSearchQuery)).MustHaveHappened();
        }

        [Fact]
        public void UserController_Index_ShouldReturnThePassedQueryAsTheModel()
        {
            var userSearchQuery = new UserSearchQuery();

            var actionResult = _userController.Index(userSearchQuery);

            actionResult.As<ViewResult>().Model.Should().BeSameAs(userSearchQuery);
        }

        [Fact]
        public void UserController_Index_ShouldReturnTheResultOfServiceCallAsViewData()
        {
            var users = new StaticPagedList<User>(new List<User>(), 1, 1, 0);
            var userSearchQuery = new UserSearchQuery();
            A.CallTo(() => _userSearchService.GetUsersPaged(userSearchQuery)).Returns(users);

            var actionResult = _userController.Index(userSearchQuery);

            _userController.ViewData["users"].Should().Be(users);
        }

        [Fact]
        public void UserController_Index_ShouldReturnViewResult()
        {
            var actionResult = _userController.Index(null);

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void UserController_SetPasswordGet_ReturnsAPartialView()
        {
            _userController.SetPassword(new User()).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void UserController_SetPasswordGet_ReturnsTheIdPassedAsTheModel()
        {
            var user = new User();
            _userController.SetPassword(user).As<PartialViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public void UserController_SetPasswordPost_ReturnsRedirectToEditUser()
        {
            var value = new User() { Id = 234 };
            A.CallTo(() => _userService.GetUser(123)).Returns(value);

            var result = _userController.SetPassword(123, "password");


            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(234); // from returned user object
        }

        [Fact]
        public void UserController_SetPasswordPost_ShouldCallAuthorisationServiceSetPassword()
        {
            var user = new User { Id = 1 };
            const string password = "password";
            A.CallTo(() => _userService.GetUser(1)).Returns(user);

            var result = _userController.SetPassword(1, password);

            A.CallTo(() => _passwordManagementService.SetPassword(user, password, password)).MustHaveHappened();
        }
    }
}