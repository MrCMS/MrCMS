using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class UserControllerTests
    {
        private UserController _userController;
        private IUserSearchService _userSearchService;
        private IUserManagementService _userService;
        private IRoleService _roleService;
        private IPasswordManagementService _passwordManagementService;
        private IGetUserCultureOptions _getUserCultureOptions;
        private IGetUserEditTabsService _getUserEditTabsService;

        public UserControllerTests()
        {
            _userSearchService = A.Fake<IUserSearchService>();
            _userService = A.Fake<IUserManagementService>();
            _roleService = A.Fake<IRoleService>();
            _passwordManagementService = A.Fake<IPasswordManagementService>();
            _getUserCultureOptions = A.Fake<IGetUserCultureOptions>();
            _getUserEditTabsService = A.Fake<IGetUserEditTabsService>();
            _userController = new UserController(_userService, _userSearchService, _roleService,
                _passwordManagementService, _getUserCultureOptions, _getUserEditTabsService);
        }

        [Fact]
        public void UserController_Index_ShouldReturnViewResult()
        {
            ActionResult actionResult = _userController.Index(null);

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void UserController_Index_ShouldCallUserServiceGetUsersPaged()
        {
            var userSearchQuery = new UserSearchQuery();

            _userController.Index(userSearchQuery);

            A.CallTo(() => _userSearchService.GetUsersPaged(userSearchQuery)).MustHaveHappened();
        }

        [Fact]
        public void UserController_Index_ShouldReturnTheResultOfServiceCallAsViewData()
        {
            var users = new StaticPagedList<User>(new List<User>(), 1, 1, 0);
            var userSearchQuery = new UserSearchQuery();
            A.CallTo(() => _userSearchService.GetUsersPaged(userSearchQuery)).Returns(users);

            ActionResult actionResult = _userController.Index(userSearchQuery);

            _userController.ViewData["users"].Should().Be(users);
        }

        [Fact]
        public void UserController_Index_ShouldReturnThePassedQueryAsTheModel()
        {
            var userSearchQuery = new UserSearchQuery();

            ActionResult actionResult = _userController.Index(userSearchQuery);

            actionResult.As<ViewResult>().Model.Should().BeSameAs(userSearchQuery);
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAViewResult()
        {
            PartialViewResult actionResult = _userController.Add();

            actionResult.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAnAddUserModel()
        {
            PartialViewResult actionResult = _userController.Add();

            actionResult.As<PartialViewResult>().Model.Should().BeOfType<AddUserModel>();
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

            ActionResult result = _userController.Add(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnAViewResultWithThePassedUserAsAModel()
        {
            var user = new User();

            ActionResult result = _userController.Edit_Get(user);

            result.As<ViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnRedirectToIndexIfIdIsInvalid()
        {
            A.CallTo(() => _userService.GetUser(1)).Returns(null);

            ActionResult result = _userController.Edit_Get(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
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
            var user = new User();

            _userController.Edit(user);

            A.CallTo(() => _userService.SaveUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_EditPost_ShouldReturnRedirectToEdit()
        {
            var user = new User { Id = 123 };

            ActionResult result = _userController.Edit(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(123);
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
            ActionResult result = _userController.SetPassword(new User { Id = 1 }, "password");

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void UserController_SetPasswordPost_ShouldCallAuthorisationServiceSetPassword()
        {
            var user = new User { Id = 1 };
            const string password = "password";

            ActionResult result = _userController.SetPassword(user, password);

            A.CallTo(() => _passwordManagementService.SetPassword(user, password, password)).MustHaveHappened();
        }
    }
}