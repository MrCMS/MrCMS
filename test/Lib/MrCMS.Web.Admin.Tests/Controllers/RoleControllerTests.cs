using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.People;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
{
    public class RoleControllerTests
    {
        public RoleControllerTests()
        {
            _roleService = A.Fake<IRoleAdminService>();
            _sut = new RoleController(_roleService) {TempData = new MockTempDataDictionary()};
        }

        private readonly IRoleAdminService _roleService;
        private readonly RoleController _sut;

        [Fact]
        public void RoleController_AddGet_ModelShouldBeAUserRole()
        {
            _sut.Add().As<PartialViewResult>().Model.Should().BeOfType<UserRole>();
        }

        [Fact]
        public void RoleController_AddGet_ReturnsAPartialViewResult()
        {
            _sut.Add().Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void RoleController_AddPost_ReturnsRedirectToActionResult()
        {
            var model = new AddRoleModel();

            _sut.Add(model).Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task RoleController_AddPost_ShouldCallAddRoleWithPassedRole()
        {
            var model = new AddRoleModel {Name = "test"};

            await _sut.Add(model);

            A.CallTo(() => _roleService.AddRole(model)).MustHaveHappened();
        }


        [Fact]
        public async Task RoleController_AddPost_ShouldRedirectToIndex()
        {
            var model = new AddRoleModel();

            var redirectToRouteResult = await _sut.Add(model);

            redirectToRouteResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task RoleController_DeleteGet_ModelShouldBeFromService()
        {
            var model = new UpdateRoleModel();
            A.CallTo(() => _roleService.GetEditModel(123)).Returns(model);

            var result = await _sut.Delete_Get(123);

            result.As<ViewResult>().Model.Should().Be(model);
        }

        [Fact]
        public async Task RoleController_DeleteGet_ReturnsViewResult()
        {
            var model = new UpdateRoleModel();
            A.CallTo(() => _roleService.GetEditModel(123)).Returns(model);

            var result = await _sut.Delete_Get(123);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task RoleController_DeleteGet_UnknownRoleReturnsRedirectToIndex()
        {
            A.CallTo(() => _roleService.GetEditModel(123)).Returns((UpdateRoleModel) null);

            var result = await _sut.Delete_Get(123);

            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task RoleController_DeletePost_CallsRoleServiceDeleteRoleOnPassedId()
        {
            await _sut.Delete(123);

            A.CallTo(() => _roleService.DeleteRole(123)).MustHaveHappened();
        }

        [Fact]
        public async Task RoleController_DeletePost_ReturnsRedirectToIndex()
        {
            var delete = await _sut.Delete(123);
            delete.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task RoleController_EditGet_IfUserRoleIsNullReturnsRedirectToIndex()
        {
            A.CallTo(() => _roleService.GetEditModel(123)).Returns((UpdateRoleModel) null);

            var editGet = await _sut.Edit_Get(123);

            editGet.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task RoleController_EditGet_IfUserRoleIsSetShouldReturnViewResult()
        {
            A.CallTo(() => _roleService.GetEditModel(123)).Returns(new UpdateRoleModel());

            var editGet = await _sut.Edit_Get(123);

            editGet.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task RoleController_EditGet_IfUserRoleIsSetViewResultModelShouldBeRolePassed()
        {
            var model = new UpdateRoleModel();
            A.CallTo(() => _roleService.GetEditModel(123)).Returns(model);

            var editGet = await _sut.Edit_Get(123);

            editGet.As<ViewResult>().Model.Should().Be(model);
        }

        [Fact]
        public async Task RoleController_EditPost_ShouldCallSaveRoleOnTheRoleService()
        {
            var model = new UpdateRoleModel();

            await _sut.Edit(model);

            A.CallTo(() => _roleService.SaveRole(model)).MustHaveHappened();
        }

        [Fact]
        public void RoleController_EditPost_ShouldRedirectToIndex()
        {
            var model = new UpdateRoleModel();

            var result = _sut.Edit(model).As<RedirectToActionResult>();

            result.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task RoleController_EditPost_ShouldReturnARedirectToActionResult()
        {
            var model = new UpdateRoleModel();

            ActionResult edit = await _sut.Edit(model);

            edit.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task RoleController_Index_CallsRoleServiceGetAllRoles()
        {
            await _sut.Index();

            A.CallTo(() => _roleService.GetAllRoles()).MustHaveHappened();
        }

        [Fact]
        public async Task RoleController_Index_ReturnsViewResult()
        {
            var index = await _sut.Index();
            index.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task RoleController_Index_ShouldReturnTheResultOfGetAllRoles()
        {
            var userRoles = new List<UserRole>();
            A.CallTo(() => _roleService.GetAllRoles()).Returns(userRoles);

            var index = await _sut.Index();
            var result = index.As<ViewResult>();

            result.Model.Should().Be(userRoles);
        }
    }
}