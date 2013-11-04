using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class UserControllerTests
    {
        private static IUserService _userService;
        private static IPasswordManagementService _passwordManagementService;
        private static IRoleService _roleService;

        [Fact]
        public void UserController_Index_ShouldReturnViewResult()
        {
            UserController userController = GetUserController();

            ActionResult actionResult = userController.Index(null);

            actionResult.Should().BeOfType<ViewResult>();
        }

        private static UserController GetUserController(IUserService userService = null, IRoleService roleService = null,
                                                        IPasswordManagementService passwordManagementService= null)
        {
            _userService = userService ?? A.Fake<IUserService>();
            _roleService = roleService ?? A.Fake<IRoleService>();
            _passwordManagementService = passwordManagementService ?? A.Fake<IPasswordManagementService>();
            var userController = new UserController(_userService, _roleService, _passwordManagementService);
            return userController;
        }

        [Fact]
        public void UserController_Index_ShouldCallUserServiceGetUsersPaged()
        {
            UserController userController = GetUserController();
            var userSearchQuery = new UserSearchQuery();

            userController.Index(userSearchQuery);

            A.CallTo(() => _userService.GetUsersPaged(userSearchQuery)).MustHaveHappened();
        }

        [Fact]
        public void UserController_Index_ShouldReturnTheResultOfServiceCallAsModel()
        {
            UserController userController = GetUserController();
            var users = new StaticPagedList<User>(new List<User>(), 1, 1, 0);
            var userSearchQuery = new UserSearchQuery();
            A.CallTo(() => _userService.GetUsersPaged(userSearchQuery)).Returns(users);

            ActionResult actionResult = userController.Index(userSearchQuery);

            actionResult.As<ViewResult>().Model.Should().BeSameAs(users);
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAViewResult()
        {
            UserController userController = GetUserController();

            PartialViewResult actionResult = userController.Add();

            actionResult.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAnAddUserModel()
        {
            UserController userController = GetUserController();

            PartialViewResult actionResult = userController.Add();

            actionResult.As<PartialViewResult>().Model.Should().BeOfType<AddUserModel>();
        }

        [Fact]
        public void UserController_AddPost_ShouldCallUserServiceSaveUser()
        {
            UserController userController = GetUserController();
            var user = new User();

            userController.Add(user);

            A.CallTo(() => _userService.AddUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_AddPost_ShouldReturnRedirectToIndex()
        {
            UserController userController = GetUserController();
            var user = new User();

            ActionResult result = userController.Add(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnAViewResultWithThePassedUserAsAModel()
        {
            UserController userController = GetUserController();
            var user = new User();

            ActionResult result = userController.Edit_Get(user);

            result.As<ViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnRedirectToIndexIfIdIsInvalid()
        {
            UserController userController = GetUserController();
            A.CallTo(() => _userService.GetUser(1)).Returns(null);

            ActionResult result = userController.Edit_Get(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void UserController_EditGet_ShouldSetViewDataForAvailableRolesAndSites()
        {
            UserController userController = GetUserController();
            var user = new User();
            var roles = new List<UserRole>();
            A.CallTo(() => _roleService.GetAllRoles()).Returns(roles);

            userController.Edit_Get(user);

            userController.ViewData["AvailableRoles"].Should().Be(roles);
        }

        [Fact]
        public void UserController_EditPost_ShouldCallSaveUser()
        {
            UserController userController = GetUserController();
            var user = new User();

            userController.Edit(user);

            A.CallTo(() => _userService.SaveUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_EditPost_ShouldReturnRedirectToEdit()
        {
            UserController userController = GetUserController();
            var user = new User{Id = 123};

            ActionResult result = userController.Edit(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void UserController_SetPasswordGet_ReturnsAPartialView()
        {
            UserController userController = GetUserController();
            userController.SetPassword(new User()).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void UserController_SetPasswordGet_ReturnsTheIdPassedAsTheModel()
        {
            UserController userController = GetUserController();
            var user = new User();
            userController.SetPassword(user).As<PartialViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public void UserController_SetPasswordPost_ReturnsRedirectToEditUser()
        {
            UserController userController = GetUserController();

            ActionResult result = userController.SetPassword(new User {Id = 1}, "password");

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void UserController_SetPasswordPost_ShouldCallAuthorisationServiceSetPassword()
        {
            UserController userController = GetUserController();

            var user = new User {Id = 1};
            const string password = "password";
            ActionResult result = userController.SetPassword(user, password);

            A.CallTo(() => _passwordManagementService.SetPassword(user, password, password)).MustHaveHappened();
        }
    }
}