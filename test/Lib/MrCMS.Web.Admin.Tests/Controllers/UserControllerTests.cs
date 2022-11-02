using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
{
    public class UserControllerTests
    {
        public UserControllerTests()
        {
            _userSearchService = A.Fake<IUserSearchService>();
            _userService = A.Fake<IUserAdminService>();
            _roleService = A.Fake<IRoleService>();
            _getUserCultureOptions = A.Fake<IGetUserCultureOptions>();
            _userImpersonationService = A.Fake<IUserImpersonationService>();
            _userManager = A.Fake<UserManager>();
            _userController = new UserController(_userService, _userSearchService, _roleService,
                _getUserCultureOptions, _userImpersonationService, _userManager)
            {
                TempData = new MockTempDataDictionary()
            };
        }

        private readonly UserController _userController;
        private readonly IUserSearchService _userSearchService;
        private readonly IUserAdminService _userService;
        private readonly IRoleService _roleService;
        private readonly IGetUserCultureOptions _getUserCultureOptions;
        private IUserImpersonationService _userImpersonationService;
        private UserManager _userManager;

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
        public async Task UserController_AddPost_ShouldCallUserServiceSaveUser()
        {
            var user = new AddUserModel();

            var result = await _userController.Add(user);

            A.CallTo(() => _userService.AddUser(user)).MustHaveHappened();

            result.ActionName.Should().Be("Edit");
        }

        [Fact]
        public void UserController_EditGet_ShouldReturnAViewResultWithTheLoadedModelAsTheViewModel()
        {
            var user = new User();
            var model = new UpdateUserModel();
            A.CallTo(() => _userService.GetUpdateModel(user)).Returns(model);

            //var result = _userController.Edit_Get(0);

            //result.As<ViewResult>().Model.Should().Be(model);
        }

        /*[Fact]
        public void UserController_EditGet_ShouldSetViewDataForAvailableRolesAndSites()
        {
            var user = new User(){Id = 1};
            var roles = new List<UserRole>();
            A.CallTo(() => _roleService.GetAllRoles()).Returns(roles);

            _userController.Edit_Get(1);

            _userController.ViewData["AvailableRoles"].Should().Be(roles);
        }*/

        [Fact]
        public async Task UserController_EditPost_ShouldCallSaveUser()
        {
            var user = new UpdateUserModel();
            List<int> roles = new List<int>();

            await _userController.Edit(user, roles);

            A.CallTo(() => _userService.SaveUser(user, roles)).MustHaveHappened();
        }

        [Fact]
        public async Task UserController_EditPost_ShouldReturnRedirectToEdit()
        {
            var model = new UpdateUserModel();
            List<int> roles = new List<int>();
            A.CallTo(() => _userService.SaveUser(model, roles)).Returns(new User {Id = 123});

            var result = await _userController.Edit(model, roles);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public async Task UserController_Index_ShouldCallUserServiceGetUsersPaged()
        {
            var userSearchQuery = new UserSearchQuery();

            await _userController.Index(userSearchQuery);

            A.CallTo(() => _userSearchService.GetUsersPaged(userSearchQuery)).MustHaveHappened();
        }

        [Fact]
        public async Task UserController_Index_ShouldReturnThePassedQueryAsTheModel()
        {
            var userSearchQuery = new UserSearchQuery();

            var actionResult = await _userController.Index(userSearchQuery);

            actionResult.As<ViewResult>().Model.Should().BeSameAs(userSearchQuery);
        }

        [Fact]
        public async Task UserController_Index_ShouldReturnTheResultOfServiceCallAsViewData()
        {
            var users = new StaticPagedList<User>(new List<User>(), 1, 1, 0);
            var userSearchQuery = new UserSearchQuery();
            A.CallTo(() => _userSearchService.GetUsersPaged(userSearchQuery)).Returns(users);

            var actionResult = await _userController.Index(userSearchQuery);

            _userController.ViewData["users"].Should().Be(users);
        }

        [Fact]
        public async Task UserController_Index_ShouldReturnViewResult()
        {
            var actionResult = await _userController.Index(null);

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task UserController_SetPasswordGet_ReturnsAPartialView()
        {
            var password = await _userController.SetPassword(0);
            password.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public async Task UserController_SetPasswordGet_ReturnsTheIdPassedAsTheModel()
        {
            var user = new User();
            var password = await _userController.SetPassword(0);
            password.As<PartialViewResult>().Model.Should().Be(user);
        }

        [Fact]
        public async Task UserController_SetPasswordPost_ReturnsRedirectToEditUser()
        {
            var result = await _userController.SetPassword(123, "password");


            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public async Task UserController_SetPasswordPost_ShouldCallAuthorisationServiceSetPassword()
        {
            const string password = "password";

            var result = await _userController.SetPassword(123, password);

            A.CallTo(() => _userService.SetPassword(123, password)).MustHaveHappened();
        }
    }
}