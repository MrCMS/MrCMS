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
        [Fact]
        public void UserController_Index_ShouldReturnViewResult()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };

            var actionResult = userController.Index();

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void UserController_Index_ShouldCallUserServiceGetAllUsers()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };

            userController.Index();

            A.CallTo(() => userService.GetAllUsers()).MustHaveHappened();
        }

        [Fact]
        public void UserController_Index_ShouldReturnTheResultOfServiceCallAsModel()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };
            var users = new List<User>();
            A.CallTo(() => userService.GetAllUsers()).Returns(users);

            var actionResult = userController.Index();

            actionResult.As<ViewResult>().Model.Should().BeSameAs(users);
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAViewResult()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };

            var actionResult = userController.Add();

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void UserController_AddGet_ShouldReturnAnAddUserModel()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };

            var actionResult = userController.Add();

            actionResult.As<ViewResult>().Model.Should().BeOfType<AddUserModel>();
        }

        [Fact]
        public void UserController_AddPost_ShouldCallUserServiceSaveUser()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };
            var user = new User();

            userController.Add(user);

            A.CallTo(() => userService.SaveUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_AddPost_ShouldReturnRedirectToIndex()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };
            var user = new User();

            var result = userController.Add(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void UserController_EditGet_ShouldCallGetUserForTheSpecifiedId()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };

            userController.Edit(1);

            A.CallTo(() => userService.GetUser(1)).MustHaveHappened();
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnAViewResultWithTheReturnedValueIfValidId()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };
            var user = new User();
            A.CallTo(() => userService.GetUser(1)).Returns(user);

            var result = userController.Edit(1);

            result.As<ViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnRedirectToIndexIfIdIsInvalid()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };
            A.CallTo(() => userService.GetUser(1)).Returns(null);

            var result = userController.Edit(1);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void UserController_EditPost_ShouldCallSaveUser()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };
            var user = new User();

            userController.Edit(user);

            A.CallTo(() => userService.SaveUser(user)).MustHaveHappened();
        }

        [Fact]
        public void UserController_EditPost_ShouldReturnRedirectToIndex()
        {
            var userService = A.Fake<IUserService>();
            var userController = new UserController(userService) { IsAjaxRequest = false };
            var user = new User();

            var result = userController.Edit(user);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }
    }
}