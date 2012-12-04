using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class RoleControllerTests
    {
        private IRoleService roleService;

        [Fact]
        public void RoleController_Index_ReturnsViewResult()
        {
            var roleController = GetRoleController();

            roleController.Index().Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void RoleController_Index_CallsRoleServiceGetAllRoles()
        {
            var roleController = GetRoleController();

            roleController.Index();

            A.CallTo(() => roleService.GetAllRoles()).MustHaveHappened();
        }

        [Fact]
        public void RoleController_Index_ShouldReturnTheResultOfGetAllRoles()
        {
            var roleController = GetRoleController();
            var userRoles = new List<UserRole>();
            A.CallTo(() => roleService.GetAllRoles()).Returns(userRoles);

            var result = roleController.Index().As<ViewResult>();

            result.Model.Should().Be(userRoles);
        }

        [Fact]
        public void RoleController_AddGet_ReturnsAViewResult()
        {
            var roleController = GetRoleController();

            roleController.Add().Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void RoleController_AddGet_ModelShouldBeAUserRole()
        {
            var roleController = GetRoleController();

            roleController.Add().As<ViewResult>().Model.Should().BeOfType<UserRole>();
        }

        [Fact]
        public void RoleController_AddPost_ReturnsRedirectToRouteResult()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Add(userRole).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void RoleController_AddPost_ShouldCallSaveRoleWithPassedRole()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Add(userRole);

            A.CallTo(() => roleService.SaveRole(userRole)).MustHaveHappened();
        }


        [Fact]
        public void RoleController_AddPost_ShouldRedirectToIndex()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            var redirectToRouteResult = roleController.Add(userRole).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_EditGet_IfUserRoleIsNullReturnsRedirectToIndex()
        {
            var roleController = GetRoleController();

            var editGet = roleController.Edit_Get(null);

            editGet.Should().BeOfType<RedirectToRouteResult>();
            editGet.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_EditGet_IfUserRoleIsSetShouldReturnViewResult()
        {
            var roleController = GetRoleController();

            var editGet = roleController.Edit_Get(new UserRole());

            editGet.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void RoleController_EditGet_IfUserRoleIsSetViewResultModelShouldBeRolePassed()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            var editGet = roleController.Edit_Get(userRole);

            editGet.As<ViewResult>().Model.Should().Be(userRole);
        }

        [Fact]
        public void RoleController_EditPost_ShouldReturnARedirectToRouteResult()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            var edit = roleController.Edit(userRole);

            edit.Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void RoleController_EditPost_ShouldCallSaveRoleOnTheRoleService()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Edit(userRole);

            A.CallTo(() => roleService.SaveRole(userRole)).MustHaveHappened();
        }

        [Fact]
        public void RoleController_EditPost_ShouldRedirectToIndex()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            var redirectToRouteResult = roleController.Edit(userRole).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_DeleteGet_NullRoleReturnsRedirectToIndex()
        {
            var roleController = GetRoleController();

            var result = roleController.Delete_Get(null);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void RoleController_DeleteGet_ReturnsViewResult()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            var result = roleController.Delete_Get(userRole);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void RoleController_DeleteGet_ModelShouldBePassedRole()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            var result = roleController.Delete_Get(userRole);

            result.As<ViewResult>().Model.Should().Be(userRole);
        }

        [Fact]
        public void RoleController_DeletePost_ReturnsRedirectToIndex()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Delete(userRole).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void RoleController_DeletePost_CallsRoleServiceDeleteRoleOnPassedRole()
        {
            var roleController = GetRoleController();
            var userRole = new UserRole();

            roleController.Delete(userRole);

            A.CallTo(() => roleService.DeleteRole(userRole)).MustHaveHappened();
        }

        [Fact]
        public void RoleController_DeletePost_DoesNotCallDeleteRoleIfUserRoleIsNull()
        {
            var roleController = GetRoleController();

            roleController.Delete(null);

            A.CallTo(() => roleService.DeleteRole(null)).MustNotHaveHappened();
        }

        private RoleController GetRoleController()
        {
            roleService = A.Fake<IRoleService>();
            return new RoleController(roleService);
        }
    }
}