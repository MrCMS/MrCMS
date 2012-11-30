using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class UserControllerTests
    {
        private static IUserService _userService;
        private static IAuthorisationService _authorisationService;
        private static IRoleService _roleService;

        [Fact]
        public void UserController_Index_ShouldReturnViewResult()
        {
            var userController = GetUserController();

            var actionResult = userController.Index();

            actionResult.Should().BeOfType<ViewResult>();
        }

        private static UserController GetUserController(IUserService userService = null, IRoleService roleService = null,
            IAuthorisationService authorisationService = null)
        {
            _userService = userService ?? A.Fake<IUserService>();
            _roleService = roleService ?? A.Fake<IRoleService>();
            _authorisationService = authorisationService ?? A.Fake<IAuthorisationService>();
            var userController = new UserController(_userService, _roleService, _authorisationService) { IsAjaxRequest = false };
            return userController;
        }

        [Fact]
        public void UserController_Index_ShouldCallUserServiceGetAllUsers()
        {
            var userController = GetUserController();

            userController.Index();

            A.CallTo(() => _userService.GetAllUsers()).MustHaveHappened();
        }

        [Fact]
        public void UserController_Index_ShouldReturnTheResultOfServiceCallAsModel()
        {
            var userController = GetUserController();
            var users = new List<User>();
            A.CallTo(() => _userService.GetAllUsers()).Returns(users);

            var actionResult = userController.Index();

            actionResult.As<ViewResult>().Model.Should().BeSameAs(users);
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAViewResult()
        {
            var userController = GetUserController();

            var actionResult = userController.Add();

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAnAddUserModel()
        {
            var userController = GetUserController();

            var actionResult = userController.Add();

            actionResult.As<ViewResult>().Model.Should().BeOfType<AddUserModel>();
        }

        [Fact]
        public void UserController_AddPost_ShouldCallUserServiceSaveUser()
        {
            var userController = GetUserController();
            var user = new User();

            userController.Add(user);

            A.CallTo(() => _userService.SaveUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_AddPost_ShouldReturnRedirectToIndex()
        {
            var userController = GetUserController();
            var user = new User();

            var result = userController.Add(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void UserController_EditGet_ShouldCallGetUserForTheSpecifiedId()
        {
            var userController = GetUserController();

            userController.Edit(1);

            A.CallTo(() => _userService.GetUser(1)).MustHaveHappened();
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnAViewResultWithTheReturnedValueIfValidId()
        {
            var userController = GetUserController();
            var user = new User();
            A.CallTo(() => _userService.GetUser(1)).Returns(user);

            var result = userController.Edit(1);

            result.As<ViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnRedirectToIndexIfIdIsInvalid()
        {
            var userController = GetUserController();
            A.CallTo(() => _userService.GetUser(1)).Returns(null);

            var result = userController.Edit(1);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void UserController_EditPost_ShouldCallSaveUser()
        {
            var userController = GetUserController();
            var user = new User();

            userController.Edit(user);

            A.CallTo(() => _userService.SaveUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_EditPost_ShouldReturnRedirectToIndex()
        {
            var userController = GetUserController();
            var user = new User();

            var result = userController.Edit(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void UserController_SetPasswordGet_ReturnsAPartialView()
        {
            var userController = GetUserController();
            userController.SetPassword(1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void UserController_SetPasswordGet_ReturnsTheIdPassedAsTheModel()
        {
            var userController = GetUserController();
            userController.SetPassword(1).As<PartialViewResult>().Model.Should().Be(1);
        }

        [Fact]
        public void UserController_SetPasswordPost_ReturnsRedirectToEditUser()
        {
            var userController = GetUserController();

            var result = userController.SetPassword(new User {Id = 1}, "password");

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void UserController_SetPasswordPost_ShouldCallAuthorisationServiceSetPassword()
        {
            var userController = GetUserController();

            var user = new User {Id = 1};
            const string password = "password";
            var result = userController.SetPassword(user, password);

            A.CallTo(() => _authorisationService.SetPassword(user, password, password)).MustHaveHappened();
        }
    }
}